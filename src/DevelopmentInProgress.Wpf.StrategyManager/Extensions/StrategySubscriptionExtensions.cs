using DevelopmentInProgress.Wpf.StrategyManager.Model;

namespace DevelopmentInProgress.Wpf.StrategyManager.Extensions
{
    public static class StrategySubscriptionExtensions
    {
        public static MarketView.Interface.TradeStrategy.StrategySubscription GetInterfaceStrategySubscription(this StrategySubscription strategySubscription)
        {
            int subscribe = 0;

            if (strategySubscription.SubscribeAccount)
            {
                subscribe += 1;
            }

            if (strategySubscription.SubscribeTrades)
            {
                subscribe += 2;
            }

            if (strategySubscription.SubscribeOrderBook)
            {
                subscribe += 4;
            }

            if (strategySubscription.SubscribeStatistics)
            {
                subscribe += 8;
            }

            var interfaceStrategySubscription = new MarketView.Interface.TradeStrategy.StrategySubscription
            {
                Symbol = strategySubscription.Symbol,
                Limit = strategySubscription.Limit,
                ApiKey = strategySubscription.ApiKey,
                SecretKey = strategySubscription.SecretKey,
                Exchange = strategySubscription.Exchange,
                Subscribe = (MarketView.Interface.TradeStrategy.Subscribe)subscribe
            };

            return interfaceStrategySubscription;
        }
    }
}
