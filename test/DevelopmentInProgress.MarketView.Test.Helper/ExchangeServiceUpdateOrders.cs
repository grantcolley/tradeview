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
    public class ExchangeServiceUpdateOrders : IExchangeService
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
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Trade>> GetTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetOpenOrdersAsync(Exchange exchange, User user, string symbol = null, long recWindow = 0, Action<Exception> exception = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<IEnumerable<Order>>();
            var orders = TestHelper.Orders;
            foreach(var order in orders)
            {
                order.ExecutedQuantity = Math.Round(order.OriginalQuantity * 0.5m, 0);
            }

            tcs.SetResult(orders);
            return tcs.Task;
        }

        public Task<OrderBook> GetOrderBookAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void SubscribeTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SubscribeOrderBook(Exchange exchange, string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SubscribeStatistics(Exchange exchange, Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SubscribeStatistics(Exchange exchange, IEnumerable<string> symbols, Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            SubscribeStatistics(exchange, callback, exception, cancellationToken);
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
