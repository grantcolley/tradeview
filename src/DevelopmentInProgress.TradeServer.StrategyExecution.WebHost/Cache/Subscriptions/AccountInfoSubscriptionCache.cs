using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public class AccountInfoSubscriptionCache : ISubscriptionCache
    {
        private readonly SubscribeAccountInfo subscribeAccountInfo;

        private bool disposed;

        public AccountInfoSubscriptionCache(IExchangeApi exchangeService)
        {
            ExchangeApi = exchangeService;

            subscribeAccountInfo = new SubscribeAccountInfo(exchangeService);
        }

        public IExchangeApi ExchangeApi { get; set; }

        public bool HasSubscriptions
        {
            get
            {
                return subscribeAccountInfo.HasSubscriptions;
            }
        }

        public int Subscriptions(Subscribes subscribe)
        {
            return subscribeAccountInfo.Subscriptions;
        }

        public void Subscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            if(strategySubscription == null)
            {
                throw new ArgumentNullException(nameof(strategySubscription));
            }

            if (tradeStrategy == null)
            {
                throw new ArgumentNullException(nameof(tradeStrategy));
            }

            if (strategySubscription.Subscribes.HasFlag(Subscribes.AccountInfo))
            {
                var accountInfo = new StrategyNotification<AccountInfoEventArgs>
                {
                    Update = tradeStrategy.SubscribeAccountInfo,
                    Exception = tradeStrategy.SubscribeAccountInfoException
                };

                subscribeAccountInfo.User.ApiKey = strategySubscription.ApiKey;
                subscribeAccountInfo.User.ApiSecret = strategySubscription.ApiKey;
                subscribeAccountInfo.Subscribe(strategyName, accountInfo);
            }
        }

        public void Unsubscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            if (strategySubscription == null)
            {
                throw new ArgumentNullException(nameof(strategySubscription));
            }

            if (tradeStrategy == null)
            {
                throw new ArgumentNullException(nameof(tradeStrategy));
            }

            if (strategySubscription.Subscribes.HasFlag(Subscribes.AccountInfo))
            {
                subscribeAccountInfo.Unsubscribe(strategyName, tradeStrategy.SubscribeAccountInfoException);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                subscribeAccountInfo.Dispose();
            }

            disposed = true;
        }
    }
}
