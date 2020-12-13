using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public class SymbolSubscriptionCache : ISubscriptionCache
    {
        private readonly SubscribeTrades subscribeTrades;
        private readonly SubscribeOrderBook subscribeOrderBook;
        private readonly SubscribeCandlesticks subscribeCandlesticks;

        private bool disposed;

        public SymbolSubscriptionCache(string symbol, int limit, TradeView.Core.Model.CandlestickInterval candlestickInterval, IExchangeApi exchangeApi)
        {
            Symbol = symbol;
            Limit = limit;
            ExchangeApi = exchangeApi;

            subscribeTrades = new SubscribeTrades(symbol, limit, ExchangeApi);
            subscribeOrderBook = new SubscribeOrderBook(symbol, limit, ExchangeApi);
            subscribeCandlesticks = new SubscribeCandlesticks(symbol, limit, ExchangeApi, candlestickInterval);
        }

        public string Symbol { get; private set; }

        public int Limit { get; private set; }

        public IExchangeApi ExchangeApi { get; set; }

        public bool HasSubscriptions
        {
            get
            {
                return subscribeTrades.HasSubscriptions
                    || subscribeOrderBook.HasSubscriptions
                    || subscribeCandlesticks.HasSubscriptions;
            }
        }

        public int Subscriptions(Subscribes subscribes)
        {
            return subscribes switch
            {
                Subscribes.Trades => subscribeTrades.Subscriptions,
                Subscribes.OrderBook => subscribeOrderBook.Subscriptions,
                Subscribes.Candlesticks => subscribeCandlesticks.Subscriptions,
                _ => throw new NotImplementedException($"{this.GetType().Name}.Subscriptions({subscribes})"),
            };
        }

        public void Subscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            if (strategySubscription == null)
            {
                throw new ArgumentNullException(nameof(strategySubscription));
            }

            if (tradeStrategy == null)
            {
                throw new ArgumentNullException(nameof(tradeStrategy));
            }

            if (strategySubscription.Subscribes.HasFlag(Subscribes.Trades))
            {
                var trades = new StrategyNotification<TradeEventArgs>
                {
                    Update = tradeStrategy.SubscribeTrades,
                    Exception = tradeStrategy.SubscribeTradesException
                };

                subscribeTrades.Subscribe(strategyName, trades);
            }

            if (strategySubscription.Subscribes.HasFlag(Subscribes.OrderBook))
            {
                var orderBook = new StrategyNotification<OrderBookEventArgs>
                {
                    Update = tradeStrategy.SubscribeOrderBook,
                    Exception = tradeStrategy.SubscribeOrderBookException
                };

                subscribeOrderBook.Subscribe(strategyName, orderBook);
            }

            if (strategySubscription.Subscribes.HasFlag(Subscribes.Candlesticks))
            {
                var candlestick = new StrategyNotification<CandlestickEventArgs>
                {
                    Update = tradeStrategy.SubscribeCandlesticks,
                    Exception = tradeStrategy.SubscribeCandlesticksException
                };

                subscribeCandlesticks.Subscribe(strategyName, candlestick);
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

            if (strategySubscription.Subscribes.HasFlag(Subscribes.Trades))
            {
                subscribeTrades.Unsubscribe(strategyName, tradeStrategy.SubscribeTradesException);
            }

            if (strategySubscription.Subscribes.HasFlag(Subscribes.OrderBook))
            {
                subscribeOrderBook.Unsubscribe(strategyName, tradeStrategy.SubscribeOrderBookException);
            }

            if (strategySubscription.Subscribes.HasFlag(Subscribes.Candlesticks))
            {
                subscribeCandlesticks.Unsubscribe(strategyName, tradeStrategy.SubscribeCandlesticksException);
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
                subscribeTrades.Dispose();
                subscribeOrderBook.Dispose();
                subscribeCandlesticks.Dispose();
            }

            disposed = true;
        }
    }
}
