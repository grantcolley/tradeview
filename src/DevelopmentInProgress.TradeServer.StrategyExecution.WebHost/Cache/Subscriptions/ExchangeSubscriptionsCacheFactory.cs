using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Enums;
using System;
using System.Collections.Concurrent;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public class ExchangeSubscriptionsCacheFactory : IExchangeSubscriptionsCacheFactory
    {
        private readonly IExchangeService exchangeService;
        private readonly ConcurrentDictionary<Exchange, IExchangeSubscriptionsCache> exchangeSubscriptionsCache;

        private bool disposed;

        public ExchangeSubscriptionsCacheFactory(IExchangeService exchangeService)
        {
            this.exchangeService = exchangeService;
            exchangeSubscriptionsCache = new ConcurrentDictionary<Exchange, IExchangeSubscriptionsCache>();
        }

        public IExchangeSubscriptionsCache GetExchangeSubscriptionsCache(Exchange exchange)
        {
            return exchangeSubscriptionsCache.GetOrAdd(exchange, new ExchangeSubscriptionsCache(exchange, exchangeService));
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
                foreach(var subscriptionsCache in exchangeSubscriptionsCache.Values)
                {
                    subscriptionsCache.Dispose();
                }
            }

            disposed = true;
        }
    }
}