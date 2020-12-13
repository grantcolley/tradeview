using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public class SubscriptionsCacheManager : ISubscriptionsCacheManager
    {
        private bool disposed;

        public SubscriptionsCacheManager(IExchangeSubscriptionsCacheFactory exchangeSubscriptionsCacheFactory)
        {
            ExchangeSubscriptionsCacheFactory = exchangeSubscriptionsCacheFactory;
        }

        public IExchangeSubscriptionsCacheFactory ExchangeSubscriptionsCacheFactory { get; private set; }

        public async Task Subscribe(Strategy strategy, ITradeStrategy tradeStrategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            var exchangeSymbolsList = (from s in strategy.StrategySubscriptions
                                  group s by s.Exchange into es
                                  select new { Exchange = es.Key, StrategySubscriptions = es.ToList() }).ToList();

            foreach(var exchangeSymbols in exchangeSymbolsList)
            {
                var exchangeSubscriptionsCache = ExchangeSubscriptionsCacheFactory.GetExchangeSubscriptionsCache(exchangeSymbols.Exchange);
                await exchangeSubscriptionsCache.Subscribe(strategy.Name, exchangeSymbols.StrategySubscriptions, tradeStrategy).ConfigureAwait(false);
            }
        }

        public void Unsubscribe(Strategy strategy, ITradeStrategy tradeStrategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            var exchangeSymbolsList = (from s in strategy.StrategySubscriptions
                                       group s by s.Exchange into es
                                       select new { Exchange = es.Key, StrategySubscriptions = es.ToList() }).ToList();

            foreach (var exchangeSymbols in exchangeSymbolsList)
            {
                var exchangeSubscriptionsCache = ExchangeSubscriptionsCacheFactory.GetExchangeSubscriptionsCache(exchangeSymbols.Exchange);
                exchangeSubscriptionsCache.Unsubscribe(strategy.Name, exchangeSymbols.StrategySubscriptions, tradeStrategy);
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
                ExchangeSubscriptionsCacheFactory.Dispose();
            }

            disposed = true;
        }
    }
}