using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Model;
using DevelopmentInProgress.TradeView.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Service
{
    public class ExchangeService : IExchangeService
    {
        private readonly IExchangeApiFactory exchangeApiFactory;
        private readonly Dictionary<Exchange, IExchangeApi> exchanges;

        public ExchangeService(IExchangeApiFactory exchangeApiFactory)
        {
            this.exchangeApiFactory = exchangeApiFactory ?? throw new ArgumentNullException(nameof(exchangeApiFactory));
            exchanges = this.exchangeApiFactory.GetExchanges();
        }

        public IExchangeApi GetExchangeApi(Exchange exchange)
        {
            return exchangeApiFactory.GetExchangeApi(exchange);
        }

        public Task<Order> PlaceOrder(Exchange exchange, User user, ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            return exchanges[exchange].PlaceOrder(user, clientOrder, recWindow, cancellationToken);
        }

        public Task<string> CancelOrderAsync(Exchange exchange, User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            return exchanges[exchange].CancelOrderAsync(user, symbol, orderId, newClientOrderId, recWindow, cancellationToken);
        }

        public Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(Exchange exchange, User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            return exchanges[exchange].GetAccountTradesAsync(user, symbol, startDate, endDate, recWindow, cancellationToken);
        }

        public Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            return exchanges[exchange].GetAggregateTradesAsync(symbol, limit, cancellationToken);
        }

        public Task<IEnumerable<Trade>> GetTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            return exchanges[exchange].GetTradesAsync(symbol, limit, cancellationToken);
        }

        public Task<IEnumerable<Candlestick>> GetCandlesticksAsync(Exchange exchange, string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = default, CancellationToken token = default)
        {
            return exchanges[exchange].GetCandlesticksAsync(symbol, interval, startTime, endTime, limit, token);
        }

        public Task<OrderBook> GetOrderBookAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            return exchanges[exchange].GetOrderBookAsync(symbol, limit, cancellationToken);
        }

        public async Task<AccountInfo> GetAccountInfoAsync(Exchange exchange, User user, CancellationToken cancellationToken)
        {
            var accountInfo = await exchanges[exchange].GetAccountInfoAsync(user, cancellationToken).ConfigureAwait(false);
            accountInfo.User.Exchange = exchange;
            return accountInfo;
        }

        public Task<IEnumerable<Symbol>> GetSymbols24HourStatisticsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            return exchanges[exchange].GetSymbols24HourStatisticsAsync(cancellationToken);
        }

        public Task<IEnumerable<Symbol>> GetSymbolsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            return exchanges[exchange].GetSymbolsAsync(cancellationToken);
        }

        public Task<IEnumerable<SymbolStats>> Get24HourStatisticsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            return exchanges[exchange].Get24HourStatisticsAsync(cancellationToken);
        }

        public Task<IEnumerable<Order>> GetOpenOrdersAsync(Exchange exchange, User user, string symbol = null, long recWindow = 0, Action<Exception> exception = null, CancellationToken cancellationToken = default)
        {
            return exchanges[exchange].GetOpenOrdersAsync(user, symbol, recWindow, exception, cancellationToken);
        }

        public Task SubscribeCandlesticks(Exchange exchange, string symbol, CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return exchanges[exchange].SubscribeCandlesticks(symbol, candlestickInterval, limit, callback, exception, cancellationToken);
        }

        public Task SubscribeOrderBook(Exchange exchange, string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return exchanges[exchange].SubscribeOrderBook(symbol, limit, callback, exception, cancellationToken);
        }

        public Task SubscribeAggregateTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return exchanges[exchange].SubscribeAggregateTrades(symbol, limit, callback, exception, cancellationToken);
        }

        public Task SubscribeTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return exchanges[exchange].SubscribeTrades(symbol, limit, callback, exception, cancellationToken);
        }

        public Task SubscribeAccountInfo(Exchange exchange, User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return exchanges[exchange].SubscribeAccountInfo(user, callback, exception, cancellationToken);
        }

        public Task SubscribeStatistics(Exchange exchange, Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return exchanges[exchange].SubscribeStatistics(callback, exception, cancellationToken);
        }

        public Task SubscribeStatistics(Exchange exchange, IEnumerable<string> symbols, Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return exchanges[exchange].SubscribeStatistics(symbols, callback, exception, cancellationToken);
        }
    }
}
