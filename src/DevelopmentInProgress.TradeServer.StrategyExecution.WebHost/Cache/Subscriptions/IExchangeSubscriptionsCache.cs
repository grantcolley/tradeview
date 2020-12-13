using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public interface IExchangeSubscriptionsCache : IDisposable
    {
        bool HasSubscriptions { get; }
        ConcurrentDictionary<string, ISubscriptionCache> Caches { get; }
        Task Subscribe(string strategyName, List<StrategySubscription> strategySubscription, ITradeStrategy tradeStrategy);
        void Unsubscribe(string strategyName, List<StrategySubscription> strategySubscription, ITradeStrategy tradeStrategy);
    }
}