using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Service
{
    public class ExchangeService : IExchangeService
    {
        private IExchangeApiFactory exchangeApiFactory;
        private Dictionary<Exchange, IExchangeApi> exchanges;

        public ExchangeService(IExchangeApiFactory exchangeApiFactory)
        {
            this.exchangeApiFactory = exchangeApiFactory;
            exchanges = this.exchangeApiFactory.GetExchanges();
        }
        
        public async Task<Order> PlaceOrder(Exchange exchange, User user, ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await exchanges[exchange].PlaceOrder(user, clientOrder, recWindow, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<string> CancelOrderAsync(Exchange exchange, User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await exchanges[exchange].CancelOrderAsync(user, symbol, orderId, newClientOrderId, recWindow, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(Exchange exchange, User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var accountTrades = await exchanges[exchange].GetAccountTradesAsync(user, symbol, startDate, endDate, recWindow, cancellationToken).ConfigureAwait(false);
            return accountTrades;
        }

        public async Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            var aggregateTrades = await exchanges[exchange].GetAggregateTradesAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            return aggregateTrades;
        }

        public async Task<IEnumerable<Trade>> GetTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            var trades = await exchanges[exchange].GetTradesAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            return trades;
        }

        public async Task<IEnumerable<Candlestick>> GetCandlesticksAsync(Exchange exchange, string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = default(int), CancellationToken token = default(CancellationToken))
        {
            var candlesticks = await exchanges[exchange].GetCandlesticksAsync(symbol, interval, startTime, endTime, limit, token).ConfigureAwait(false);
            return candlesticks;
        }

        public async Task<OrderBook> GetOrderBookAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            var orderBook = await exchanges[exchange].GetOrderBookAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            return orderBook;
        }

        public void SubscribeCandlesticks(Exchange exchange, string symbol, CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchanges[exchange].SubscribeCandlesticks(symbol, candlestickInterval, limit, callback, exception, cancellationToken);
        }

        public void SubscribeOrderBook(Exchange exchange, string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchanges[exchange].SubscribeOrderBook(symbol, limit, callback, exception, cancellationToken);
        }

        public void SubscribeAggregateTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchanges[exchange].SubscribeAggregateTrades(symbol, limit, callback, exception, cancellationToken);
        }

        public void SubscribeTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchanges[exchange].SubscribeTrades(symbol, limit, callback, exception, cancellationToken);
        }

        public void SubscribeAccountInfo(Exchange exchange, User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchanges[exchange].SubscribeAccountInfo(user, callback, exception, cancellationToken);
        }

        public void SubscribeStatistics(Exchange exchange, Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchanges[exchange].SubscribeStatistics(callback, exception, cancellationToken);
        }

        public async Task<AccountInfo> GetAccountInfoAsync(Exchange exchange, User user, CancellationToken cancellationToken)
        {
            var accountInfo = await exchanges[exchange].GetAccountInfoAsync(user, cancellationToken).ConfigureAwait(false);
            accountInfo.User.Exchange = exchange;
            return accountInfo;
        }

        public async Task<IEnumerable<Symbol>> GetSymbolsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            return await exchanges[exchange].GetSymbolsAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<SymbolStats>> Get24HourStatisticsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            return await exchanges[exchange].Get24HourStatisticsAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Order>> GetOpenOrdersAsync(Exchange exchange, User user, string symbol = null, long recWindow = 0, Action<Exception> exception = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await exchanges[exchange].GetOpenOrdersAsync(user, symbol, recWindow, exception, cancellationToken).ConfigureAwait(false);
        }
    }
}
