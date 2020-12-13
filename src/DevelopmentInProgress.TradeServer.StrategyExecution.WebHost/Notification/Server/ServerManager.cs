using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.TradeStrategy;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Strategy;
using DevelopmentInProgress.TradeView.Core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Server
{
    public class ServerManager : IServerManager, IDisposable
    {
        private readonly IBatchNotification<ServerNotification> serverBatchNotificationPublisher;
        private readonly ITradeStrategyCacheManager tradeStrategyCacheManager;
        private readonly StrategyNotificationHub strategyNotificationHub;
        private readonly ServerNotificationHub serverNotificationHub;
        private readonly SemaphoreSlim notificationSemaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly List<IDisposable> disposables;
        private bool disposed;

        public ServerManager(IServerMonitor serverMonitor,
            IBatchNotification<ServerNotification> serverBatchNotificationPublisher,
            ITradeStrategyCacheManager tradeStrategyCacheManager,
            StrategyNotificationHub strategyNotificationHub,
            ServerNotificationHub serverNotificationHub)
        {
            ServerMonitor = serverMonitor;

            this.serverBatchNotificationPublisher = serverBatchNotificationPublisher;
            this.tradeStrategyCacheManager = tradeStrategyCacheManager;
            this.strategyNotificationHub = strategyNotificationHub;
            this.serverNotificationHub = serverNotificationHub;

            disposables = new List<IDisposable>();

            ObserverTradeStrategyCacheManager();
            ObserverStrategyNotificationHub();
            ObserverServerNotificationHub();
        }

        public IServerMonitor ServerMonitor { get; private set; }

        public void Shutdown()
        {
            tradeStrategyCacheManager.StopStrategies();

            ServerMonitor.Stopped = DateTime.Now;
            ServerMonitor.StoppedBy = Environment.UserName;

            var tasks = new List<Task>(strategyNotificationHub.CloseAndDisposeWebSockets());
            tasks.AddRange(serverNotificationHub.CloseAndDisposeWebSockets());
            Task.WaitAll(tasks.ToArray());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }

                notificationSemaphoreSlim.Dispose();
            }

            disposed = true;
        }

        private void ObserverTradeStrategyCacheManager()
        {
            var tradeStrategyCacheManagerObservable = Observable.FromEventPattern<ServerNotificationEventArgs>(
                eventHandler => tradeStrategyCacheManager.ServerNotification += eventHandler,
                eventHandler => tradeStrategyCacheManager.ServerNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            var tradeStrategyCacheManagerSubscription = tradeStrategyCacheManagerObservable.Subscribe(args =>
            {
                OnNotification();
            });

            disposables.Add(tradeStrategyCacheManagerSubscription);
        }

        private void ObserverStrategyNotificationHub()
        {
            var strategyNotificationHubObservable = Observable.FromEventPattern<ServerNotificationEventArgs>(
                eventHandler => strategyNotificationHub.ServerNotification += eventHandler,
                eventHandler => strategyNotificationHub.ServerNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            var strategyNotificationHubObservableSubscription = strategyNotificationHubObservable.Subscribe(args =>
            {
                OnNotification();
            });

            disposables.Add(strategyNotificationHubObservableSubscription);
        }

        private void ObserverServerNotificationHub()
        {
            var serverNotificationHubObservable = Observable.FromEventPattern<ServerNotificationEventArgs>(
                eventHandler => serverNotificationHub.ServerNotification += eventHandler,
                eventHandler => serverNotificationHub.ServerNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            var serverNotificationHubObservableSubscription = serverNotificationHubObservable.Subscribe(args =>
            {
                OnNotification();
            });

            disposables.Add(serverNotificationHubObservableSubscription);
        }

        private void OnNotification()
        {
            notificationSemaphoreSlim.Wait();

            try
            {
                List<ServerStrategy> strategyServers = new List<ServerStrategy>();

                var strategies = tradeStrategyCacheManager.GetStrategies();
                var serverInfo = strategyNotificationHub.GetServerInfo();

                static ServerStrategy f(ServerStrategy s, Socket.Messages.ChannelInfo c)
                {
                    s.Connections.AddRange(c.Connections.Select(conn => new ServerStrategyConnection
                    {
                        Connection = conn.Name
                    }));

                    return s;
                }

                var serverStrategies = strategies.Select(s => new ServerStrategy { Strategy = s, Started = s.Started, StartedBy = s.StartedBy }).ToList();

                _ = (from s in serverStrategies
                     join c in serverInfo.Channels on s.Strategy.Name equals c.Name
                     select f(s, c)).ToList();

                var serverNotification = ServerMonitor.GetServerNotification(serverStrategies);

                serverBatchNotificationPublisher.AddNotification(serverNotification);
            }
            finally
            {
                notificationSemaphoreSlim.Release();
            }
        }
    }
}
