using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Server;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.TradeStrategy
{
    public class TradeStrategyCacheManager : ServerNotificationBase, ITradeStrategyCacheManager
    {
        private readonly ConcurrentDictionary<string, ITradeStrategy> tradeStrategies;

        public TradeStrategyCacheManager()
        {
            tradeStrategies = new ConcurrentDictionary<string, ITradeStrategy>();
        }

        public List<Strategy> GetStrategies()
        {
            return tradeStrategies.Values.Select(s => s.Strategy).ToList();
        }

        public bool TryGetTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy)
        {
            return tradeStrategies.TryGetValue(strategyName, out tradeStrategy);
        }

        public bool TryAddTradeStrategy(string strategyName, ITradeStrategy tradeStrategy)
        {
            if (tradeStrategies.TryAdd(strategyName, tradeStrategy))
            {
                OnServerNotification();
                return true;
            }

            return false;
        }

        public bool TryRemoveTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy)
        {
            if (tradeStrategies.TryRemove(strategyName, out tradeStrategy))
            {
                OnServerNotification();
                return true;
            }

            return false;
        }

        public async Task StopStrategy(string strategyName, string parameters)
        {
            if (tradeStrategies.TryGetValue(strategyName, out ITradeStrategy tradeStrategy))
            {
                await tradeStrategy.TryStopStrategy(parameters).ConfigureAwait(false);

                OnServerNotification();
            }
        }

        public async Task UpdateStrategy(string strategyName, string parameters)
        {
            if (tradeStrategies.TryGetValue(strategyName, out ITradeStrategy tradeStrategy))
            {
                await tradeStrategy.TryUpdateStrategyAsync(parameters).ConfigureAwait(false);

                OnServerNotification();
            }
        }

        public void StopStrategies()
        {
            var strategies = tradeStrategies.Values;
            var tasks = strategies.Select(s => s.TryStopStrategy(string.Empty));
            Task.WhenAll(tasks);
        }
    }
}
