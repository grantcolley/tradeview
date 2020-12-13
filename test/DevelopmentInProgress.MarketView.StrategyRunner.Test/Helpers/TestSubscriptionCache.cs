using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestSubscriptionCache : ISubscriptionCache
    {
        public IExchangeApi ExchangeApi => throw new System.NotImplementedException();

        public bool HasSubscriptions => throw new System.NotImplementedException();

        public void Dispose()
        {
        }

        public void Subscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            throw new System.NotImplementedException();
        }

        public int Subscriptions(Subscribes subscribe)
        {
            throw new System.NotImplementedException();
        }

        public void Unsubscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            throw new System.NotImplementedException();
        }
    }
}
