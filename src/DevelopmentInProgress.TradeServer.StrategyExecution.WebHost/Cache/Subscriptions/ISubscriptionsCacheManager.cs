using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public interface ISubscriptionsCacheManager : IDisposable
    {
        IExchangeSubscriptionsCacheFactory ExchangeSubscriptionsCacheFactory { get; }
        Task Subscribe(Strategy strategy, ITradeStrategy tradeStrategy);
        void Unsubscribe(Strategy strategy, ITradeStrategy tradeStrategy);
    }
}