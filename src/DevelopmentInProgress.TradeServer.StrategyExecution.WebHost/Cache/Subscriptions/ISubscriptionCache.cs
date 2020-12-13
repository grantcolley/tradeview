using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public interface ISubscriptionCache : IDisposable
    {
        IExchangeApi ExchangeApi { get; }
        bool HasSubscriptions { get; }
        int Subscriptions(Subscribes subscribe);
        void Subscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy);
        void Unsubscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy);
    }
}