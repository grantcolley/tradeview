using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.Enums;

namespace DevelopmentInProgress.MarketView.Test.Helper
{
    public class ExchangeServiceSubscribeOrderBookAggregateTrades : IExchangeService
    {
        public Task<string> CancelOrderAsync(Exchange exchange, User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SymbolStats>> Get24HourStatisticsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<AccountInfo> GetAccountInfoAsync(Exchange exchange, User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<IEnumerable<AggregateTrade>>();
            tcs.SetResult(TestHelper.AggregateTrades);
            return tcs.Task;
        }

        public Task<IEnumerable<Trade>> GetTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<IEnumerable<Trade>>();
            tcs.SetResult(TestHelper.Trades);
            return tcs.Task;
        }

        public Task<IEnumerable<Order>> GetOpenOrdersAsync(Exchange exchange, User user, string symbol = null, long recWindow = 0, Action<Exception> exception = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<OrderBook> GetOrderBookAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<OrderBook>();
            tcs.SetResult(TestHelper.OrderBook);
            return tcs.Task;
        }

        public Task<IEnumerable<Symbol>> GetSymbolsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Order> PlaceOrder(Exchange exchange, User user, ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Candlestick>> GetCandlesticksAsync(Exchange exchange, string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = default(int), CancellationToken token = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void SubscribeAccountInfo(Exchange exchange, User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAggregateTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            callback.Invoke(new TradeEventArgs { Trades = TestHelper.AggregateTradesUpdated });
        }

        public void SubscribeTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            callback.Invoke(new TradeEventArgs { Trades = TestHelper.TradesUpdated });
        }

        public void SubscribeOrderBook(Exchange exchange, string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            callback.Invoke(new OrderBookEventArgs { OrderBook = TestHelper.OrderBookUpdated });
        }

        public void SubscribeStatistics(Exchange exchange, Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(Exchange exchange, User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void SubscribeCandlesticks(Exchange exchange, string symbol, CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Symbol>> GetSymbols24HourStatisticsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
