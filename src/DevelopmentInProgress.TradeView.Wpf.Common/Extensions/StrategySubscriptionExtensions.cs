using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Core.Extensions;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class StrategySubscriptionExtensions
    {
        public static Core.TradeStrategy.StrategySubscription ToCoreStrategySubscription(this StrategySubscription strategySubscription)
        {
            if (strategySubscription == null)
            {
                throw new ArgumentNullException(nameof(strategySubscription));
            }

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

            if (strategySubscription.SubscribeCandlesticks)
            {
                subscribe += 8;
            }

            var coreStrategySubscription = new Core.TradeStrategy.StrategySubscription
            {
                AccountName = strategySubscription.AccountName,
                Symbol = strategySubscription.Symbol,
                Limit = strategySubscription.Limit,
                ApiKey = strategySubscription.ApiKey,
                SecretKey = strategySubscription.SecretKey,
                ApiPassPhrase = strategySubscription.ApiPassPhrase,
                Exchange = strategySubscription.Exchange,
                Subscribes = (Core.TradeStrategy.Subscribes)subscribe,
                CandlestickInterval = strategySubscription.CandlestickInterval.GetCandlestickInterval()
            };

            return coreStrategySubscription;
        }

        public static StrategySubscription ToWpfStrategySubscription(this Core.TradeStrategy.StrategySubscription coreStrategySubscription)
        {
            if (coreStrategySubscription == null)
            {
                throw new ArgumentNullException(nameof(coreStrategySubscription));
            }

            var strategySubScription = new StrategySubscription
            {
                AccountName = coreStrategySubscription.AccountName,
                Symbol = coreStrategySubscription.Symbol,
                Limit = coreStrategySubscription.Limit,
                ApiKey = coreStrategySubscription.ApiKey,
                ApiPassPhrase = coreStrategySubscription.ApiPassPhrase,
                SecretKey = coreStrategySubscription.SecretKey,
                Exchange = coreStrategySubscription.Exchange,
                CandlestickInterval = coreStrategySubscription.CandlestickInterval.ToString()
            };

            if ((coreStrategySubscription.Subscribes & Core.TradeStrategy.Subscribes.AccountInfo) == Core.TradeStrategy.Subscribes.AccountInfo)
            {
                strategySubScription.SubscribeAccount = true;
            }

            if ((coreStrategySubscription.Subscribes & Core.TradeStrategy.Subscribes.Trades) == Core.TradeStrategy.Subscribes.Trades)
            {
                strategySubScription.SubscribeTrades = true;
            }

            if ((coreStrategySubscription.Subscribes & Core.TradeStrategy.Subscribes.OrderBook) == Core.TradeStrategy.Subscribes.OrderBook)
            {
                strategySubScription.SubscribeOrderBook = true;
            }

            if ((coreStrategySubscription.Subscribes & Core.TradeStrategy.Subscribes.Candlesticks) == Core.TradeStrategy.Subscribes.Candlesticks)
            {
                strategySubScription.SubscribeCandlesticks = true;
            }

            return strategySubScription;
        }
    }
}
