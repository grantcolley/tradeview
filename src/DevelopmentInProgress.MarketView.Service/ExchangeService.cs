using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Service
{
    public class ExchangeService : IExchangeService
    {
        private IExchangeApi exchangeApi;

        public ExchangeService(IExchangeApi exchangeApi)
        {
            this.exchangeApi = exchangeApi;
        }

        public async Task<Order> PlaceOrder(User user, ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await exchangeApi.PlaceOrder(user, clientOrder, recWindow, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<string> CancelOrderAsync(User user, string symbol, long orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await exchangeApi.CancelOrderAsync(user, symbol, orderId, newClientOrderId, recWindow, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var accountTrades = await exchangeApi.GetAccountTradesAsync(user, symbol, startDate, endDate, recWindow, cancellationToken).ConfigureAwait(false);
            return accountTrades;
        }

        public async Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var aggregateTrades = await exchangeApi.GetAggregateTradesAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            return aggregateTrades;
        }

        public async Task<IEnumerable<Trade>> GetTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var trades = await exchangeApi.GetTradesAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            return trades;
        }

        public async Task<IEnumerable<Candlestick>> GetCandlesticksAsync(string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = default(int), CancellationToken token = default(CancellationToken))
        {
            var candlesticks = await exchangeApi.GetCandlesticksAsync(symbol, interval, startTime, endTime, limit, token).ConfigureAwait(false);
            return candlesticks;
        }

        public async Task<OrderBook> GetOrderBookAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var orderBook = await exchangeApi.GetOrderBookAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            return orderBook;
        }

        public void SubscribeCandlesticks(string symbol, CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeCandlesticks(symbol, candlestickInterval, limit, callback, exception, cancellationToken);
        }

        public void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeOrderBook(symbol, limit, callback, exception, cancellationToken);
        }

        public void SubscribeAggregateTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeAggregateTrades(symbol, limit, callback, exception, cancellationToken);
        }

        public void SubscribeTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeTrades(symbol, limit, callback, exception, cancellationToken);
        }

        public void SubscribeAccountInfo(User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeAccountInfo(user, callback, exception, cancellationToken);
        }

        public void SubscribeStatistics(Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeStatistics(callback, exception, cancellationToken);
        }

        public async Task<AccountInfo> GetAccountInfoAsync(User user, CancellationToken cancellationToken)
        {
            return await exchangeApi.GetAccountInfoAsync(user, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken)
        {
            return await exchangeApi.GetSymbolsAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<SymbolStats>> Get24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            return await exchangeApi.Get24HourStatisticsAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Order>> GetOpenOrdersAsync(User user, string symbol = null, long recWindow = 0, Action<Exception> exception = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await exchangeApi.GetOpenOrdersAsync(user, symbol, recWindow, exception, cancellationToken).ConfigureAwait(false);
        }
    }
}
