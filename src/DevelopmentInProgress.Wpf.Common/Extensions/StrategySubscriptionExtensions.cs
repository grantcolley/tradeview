using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.MarketView.Interface.Extensions;

namespace DevelopmentInProgress.Wpf.Common.Extensions
{
    public static class StrategySubscriptionExtensions
    {
        public static MarketView.Interface.Strategy.StrategySubscription GetInterfaceStrategySubscription(this StrategySubscription strategySubscription)
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

            if (strategySubscription.SubscribeCandlesticks)
            {
                subscribe += 16;
            }

            var interfaceStrategySubscription = new MarketView.Interface.Strategy.StrategySubscription
            {
                Symbol = strategySubscription.Symbol,
                Limit = strategySubscription.Limit,
                ApiKey = strategySubscription.ApiKey,
                SecretKey = strategySubscription.SecretKey,
                Exchange = strategySubscription.Exchange,
                Subscribe = (MarketView.Interface.Strategy.Subscribe)subscribe,
                CandlestickInterval = strategySubscription.CandlestickInterval.GetCandlestickInterval()
            };

            return interfaceStrategySubscription;
        }
    }
}
