using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Server;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.TradeStrategy
{
    public interface ITradeStrategyCacheManager : IServerNotification
    {
        bool TryAddTradeStrategy(string strategyName, ITradeStrategy tradeStrategy);
        bool TryRemoveTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy);
        bool TryGetTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy);
        Task StopStrategy(string strategyName, string parameters);
        Task UpdateStrategy(string strategyName, string parameters);
        List<Strategy> GetStrategies();
        void StopStrategies();
    }
}