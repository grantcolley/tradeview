using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.TradeStrategy;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Utilities;
using DevelopmentInProgress.TradeView.Core.Extensions;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost
{
    public class StrategyRunner : IStrategyRunner
    {
        private readonly IBatchNotification<StrategyNotification> strategyLogger;
        private readonly IBatchNotification<StrategyNotification> strategyAccountInfoPublisher;
        private readonly IBatchNotification<StrategyNotification> strategyCustomNotificationPublisher;
        private readonly IBatchNotification<StrategyNotification> strategyNotificationPublisher;
        private readonly IBatchNotification<StrategyNotification> strategyOrderBookPublisher;
        private readonly IBatchNotification<StrategyNotification> strategyTradePublisher;
        private readonly IBatchNotification<StrategyNotification> strategyStatisticsPublisher;
        private readonly IBatchNotification<StrategyNotification> strategyCandlesticksPublisher;
        private readonly IBatchNotification<StrategyNotification> strategyParameterUpdatePublisher;
        private readonly ISubscriptionsCacheManager subscriptionsCacheManager;
        private readonly ITradeStrategyCacheManager tradeStrategyCacheManager;

        private CancellationToken cancellationToken;

        public StrategyRunner(
            IBatchNotificationFactory<StrategyNotification> batchNotificationFactory, 
            ISubscriptionsCacheManager subscriptionsCacheManager, 
            ITradeStrategyCacheManager tradeStrategyCacheManager)
        {
            if (batchNotificationFactory == null)
            {
                throw new ArgumentNullException(nameof(batchNotificationFactory));
            }

            this.subscriptionsCacheManager = subscriptionsCacheManager ?? throw new ArgumentNullException(nameof(subscriptionsCacheManager));
            this.tradeStrategyCacheManager = tradeStrategyCacheManager ?? throw new ArgumentNullException(nameof(tradeStrategyCacheManager));

            strategyLogger = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyLogger);
            strategyAccountInfoPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyAccountInfoPublisher);
            strategyCustomNotificationPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyCustomNotificationPublisher);
            strategyNotificationPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyNotificationPublisher);
            strategyOrderBookPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyOrderBookPublisher);
            strategyTradePublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyTradePublisher);
            strategyStatisticsPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyStatisticsPublisher);
            strategyCandlesticksPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyCandlesticksPublisher);
            strategyParameterUpdatePublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyParameterUpdatePublisher);
        }

        public async Task<Strategy> RunAsync(Strategy strategy, string localPath, CancellationToken cancellationToken)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            this.cancellationToken = cancellationToken;

            try
            {
                strategy.Status = StrategyStatus.Initialising;

                Notify(NotificationLevel.Information, NotificationEventId.RunAsync, strategy, "Initialising strategy");

                if (string.IsNullOrWhiteSpace(strategy.TargetAssembly))
                {
                    Notify(NotificationLevel.Error, NotificationEventId.RunAsync, strategy, "No TargetAssembly");
                    return strategy;
                }

                if (string.IsNullOrWhiteSpace(strategy.TargetType))
                {
                    Notify(NotificationLevel.Error, NotificationEventId.RunAsync, strategy, "No TargetType");
                    return strategy;
                }

                return await RunStrategyAsync(strategy, localPath).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Notify(NotificationLevel.Error, NotificationEventId.RunAsync, strategy, ex.ToString());
                throw;
            }
        }

        internal async Task<Strategy> RunStrategyAsync(Strategy strategy, string localPath)
        {
            ITradeStrategy tradeStrategy = null;

            try
            {
                Notify(NotificationLevel.Information, NotificationEventId.RunStrategyAsync, strategy, $"Loading {strategy.Name}");

                var dependencies = GetAssemblies(localPath);

                var assemblyLoader = new AssemblyLoader(localPath, dependencies);
                var assembly = assemblyLoader.LoadFromMemoryStream(Path.Combine(localPath, strategy.TargetAssembly));
                var type = assembly.GetType(strategy.TargetType);
                dynamic obj = Activator.CreateInstance(type);

                tradeStrategy = (ITradeStrategy)obj;

                tradeStrategy.StrategyNotificationEvent += StrategyNotificationEvent;
                tradeStrategy.StrategyAccountInfoEvent += StrategyAccountInfoEvent;
                tradeStrategy.StrategyOrderBookEvent += StrategyOrderBookEvent;
                tradeStrategy.StrategyTradeEvent += StrategyTradeEvent;
                tradeStrategy.StrategyStatisticsEvent += StrategyStatisticsEvent;
                tradeStrategy.StrategyCandlesticksEvent += StrategyCandlesticksEvent;
                tradeStrategy.StrategyParameterUpdateEvent += StrategyParameterUpdateEvent;
                tradeStrategy.StrategyCustomNotificationEvent += StrategyCustomNotificationEvent;

                tradeStrategy.SetStrategy(strategy);

                strategy.Status = StrategyStatus.Running;

                if(tradeStrategyCacheManager.TryAddTradeStrategy(strategy.Name, tradeStrategy))
                {
                    Notify(NotificationLevel.Information, NotificationEventId.RunStrategyAsync, strategy, $"Subscribing {strategy.Name}");

                    await subscriptionsCacheManager.Subscribe(strategy, tradeStrategy).ConfigureAwait(false);

                    Notify(NotificationLevel.Information, NotificationEventId.RunStrategyAsync, strategy, $"Running {strategy.Name}");

                    var result = await tradeStrategy.RunAsync(cancellationToken).ConfigureAwait(false);

                    if(!tradeStrategyCacheManager.TryRemoveTradeStrategy(strategy.Name, out ITradeStrategy ts))
                    {
                        Notify(NotificationLevel.Error, NotificationEventId.RunStrategyAsync, strategy, $"Failed to remove {strategy.Name} from the cache manager.");
                    }
                }
                else
                {
                    Notify(NotificationLevel.Error, NotificationEventId.RunStrategyAsync, strategy, $"Failed to add {strategy.Name} to the cache manager.");
                }
            }
            finally
            {
                if(tradeStrategy != null)
                {
                    subscriptionsCacheManager.Unsubscribe(strategy, tradeStrategy);

                    tradeStrategy.StrategyNotificationEvent -= StrategyNotificationEvent;
                    tradeStrategy.StrategyAccountInfoEvent -= StrategyAccountInfoEvent;
                    tradeStrategy.StrategyOrderBookEvent -= StrategyOrderBookEvent;
                    tradeStrategy.StrategyTradeEvent -= StrategyTradeEvent;
                    tradeStrategy.StrategyParameterUpdateEvent -= StrategyParameterUpdateEvent;
                    tradeStrategy.StrategyCustomNotificationEvent -= StrategyCustomNotificationEvent;
                }

                // TODO: Unload target assembly and it's dependencies from memory and delete them.
            }

            return strategy;
        }

        private void StrategyNotificationEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyNotificationPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyParameterUpdateEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyParameterUpdatePublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyAccountInfoEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyAccountInfoPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyOrderBookEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyOrderBookPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyTradeEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyTradePublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyStatisticsEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyStatisticsPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyCandlesticksEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyCandlesticksPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyCustomNotificationEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyCustomNotificationPublisher.AddNotification(e.StrategyNotification);
        }

        private void Notify(NotificationLevel notificationLevel, int notificationEvent, Strategy strategy, string message = "")
        {
            var strategyNotification = strategy.GetNotification(notificationLevel, notificationEvent, message);
            strategyNotificationPublisher.AddNotification(strategyNotification);
            strategyLogger.AddNotification(strategyNotification);
        }

        private static IList<string> GetAssemblies(string localPath)
        {
            var dependencies = new List<string>();
            var files = Directory.GetFiles(localPath); 

            foreach (string filePath in files)
            {
                if(filePath.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var filePathSplit = filePath.Split('\\');
                var fileName = filePathSplit[^1];
                var name = fileName.Substring(0, fileName.LastIndexOf('.'));
                dependencies.Add(name);
            }

            return dependencies;
        }
    }
}
