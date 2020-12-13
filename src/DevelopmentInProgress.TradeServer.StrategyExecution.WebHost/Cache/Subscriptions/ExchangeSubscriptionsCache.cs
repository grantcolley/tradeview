using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Model;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public class ExchangeSubscriptionsCache : IExchangeSubscriptionsCache
    {
        private readonly IExchangeService exchangeService;
        private readonly Exchange exchange;
        private bool disposed;

        public ExchangeSubscriptionsCache(Exchange exchange, IExchangeService exchangeService)
        {
            this.exchange = exchange;
            this.exchangeService = exchangeService;

            Caches = new ConcurrentDictionary<string, ISubscriptionCache>();
        }

        public ConcurrentDictionary<string, ISubscriptionCache> Caches { get; private set; }

        public bool HasSubscriptions
        {
            get
            {
                return (from c in Caches.Values where c.HasSubscriptions select c).Any();
            }
        }

        public async Task Subscribe(string strategyName, List<StrategySubscription> strategySubscriptions, ITradeStrategy tradeStrategy)
        {
            if (strategySubscriptions == null)
            {
                throw new ArgumentNullException(nameof(strategySubscriptions));
            }

            if (tradeStrategy == null)
            {
                throw new ArgumentNullException(nameof(tradeStrategy));
            }

            await tradeStrategy.AddExchangeService(strategySubscriptions, exchange, exchangeService).ConfigureAwait(false);

            var exchangeApi = exchangeService.GetExchangeApi(exchange);

            foreach (var strategySubscription in strategySubscriptions)
            {
                if (strategySubscription.Subscribes.HasFlag(Subscribes.Trades)
                    || strategySubscription.Subscribes.HasFlag(Subscribes.OrderBook)
                    || strategySubscription.Subscribes.HasFlag(Subscribes.Candlesticks))
                {
                    if (!Caches.TryGetValue(strategySubscription.Symbol, out ISubscriptionCache symbolCache))
                    {
                        symbolCache = new SymbolSubscriptionCache(strategySubscription.Symbol, strategySubscription.Limit, strategySubscription.CandlestickInterval, exchangeApi);
                        Caches.TryAdd(strategySubscription.Symbol, symbolCache);
                    }

                    symbolCache.Subscribe(strategyName, strategySubscription, tradeStrategy);
                }

                if (strategySubscription.Subscribes.HasFlag(Subscribes.AccountInfo))
                {
                    var user = new User
                    {
                        ApiKey = strategySubscription.ApiKey,
                        ApiSecret = strategySubscription.SecretKey,
                        ApiPassPhrase = strategySubscription.ApiPassPhrase,
                        Exchange = strategySubscription.Exchange
                    };

                    var accountInfo = await exchangeService.GetAccountInfoAsync(exchange, user, new CancellationToken()).ConfigureAwait(false);

                    tradeStrategy.SubscribeAccountInfo(new AccountInfoEventArgs { AccountInfo = accountInfo });

                    if (!Caches.TryGetValue(strategySubscription.ApiKey, out ISubscriptionCache accountInfoCache))
                    {
                        accountInfoCache = new AccountInfoSubscriptionCache(exchangeApi);
                        Caches.TryAdd(strategySubscription.ApiKey, accountInfoCache);
                    }

                    accountInfoCache.Subscribe(strategyName, strategySubscription, tradeStrategy);
                }
            }
        }

        public void Unsubscribe(string strategyName, List<StrategySubscription> strategySubscriptions, ITradeStrategy tradeStrategy)
        {
            if (strategySubscriptions == null)
            {
                throw new ArgumentNullException(nameof(strategySubscriptions));
            }

            foreach (var strategySubscription in strategySubscriptions)
            {
                if (strategySubscription.Subscribes.HasFlag(Subscribes.Trades)
                    || strategySubscription.Subscribes.HasFlag(Subscribes.OrderBook)
                    || strategySubscription.Subscribes.HasFlag(Subscribes.Candlesticks))
                {
                    Unsubscribe(strategyName, strategySubscription, strategySubscription.Symbol, tradeStrategy);
                }

                if (strategySubscription.Subscribes.HasFlag(Subscribes.AccountInfo))
                {
                    Unsubscribe(strategyName, strategySubscription, strategySubscription.ApiKey, tradeStrategy);
                }
            }
        }

        private void Unsubscribe(string strategyName, StrategySubscription symbol, string cacheKey, ITradeStrategy tradeStrategy)
        {
            if (Caches.TryGetValue(cacheKey, out ISubscriptionCache cache))
            {
                cache.Unsubscribe(strategyName, symbol, tradeStrategy);

                if (!cache.HasSubscriptions)
                {
                    if (Caches.TryRemove(cacheKey, out ISubscriptionCache cacheDispose))
                    {
                        cacheDispose.Dispose();
                    }
                }
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
                foreach (var cache in Caches)
                {
                    cache.Value.Dispose();
                }

                Caches.Clear();
            }

            disposed = true;
        }
    }
}
