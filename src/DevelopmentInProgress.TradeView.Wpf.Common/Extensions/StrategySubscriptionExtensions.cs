using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Interface.Extensions;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class StrategySubscriptionExtensions
    {
        public static Interface.Strategy.StrategySubscription GetInterfaceStrategySubscription(this StrategySubscription strategySubscription)
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

            var interfaceStrategySubscription = new Interface.Strategy.StrategySubscription
            {
                Symbol = strategySubscription.Symbol,
                Limit = strategySubscription.Limit,
                ApiKey = strategySubscription.ApiKey,
                SecretKey = strategySubscription.SecretKey,
                Exchange = strategySubscription.Exchange,
                Subscribe = (Interface.Strategy.Subscribe)subscribe,
                CandlestickInterval = strategySubscription.CandlestickInterval.GetCandlestickInterval()
            };

            return interfaceStrategySubscription;
        }
    }
}
