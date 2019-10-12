using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Interface.Interfaces
{
    public interface IExchangeService
    {
        Task<Order> PlaceOrder(Exchange exchange, User user, ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> CancelOrderAsync(Exchange exchange, User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken));
        Task<AccountInfo> GetAccountInfoAsync(Exchange exchange, User user, CancellationToken cancellationToken);
        Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(Exchange exchange, User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Symbol>> GetSymbolsAsync(Exchange exchange, CancellationToken cancellationToken);
        Task<IEnumerable<SymbolStats>> Get24HourStatisticsAsync(Exchange exchange, CancellationToken cancellationToken);
        Task<OrderBook> GetOrderBookAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<Trade>> GetTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<Order>> GetOpenOrdersAsync(Exchange exchange, User user, string symbol = null, long recWindow = 0, Action<Exception> exception = default(Action<Exception>), CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Candlestick>> GetCandlesticksAsync(Exchange exchange, string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = default(int), CancellationToken token = default(CancellationToken));
        void SubscribeCandlesticks(Exchange exchange, string symbol, CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeStatistics(Exchange exchange, Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeOrderBook(Exchange exchange, string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeAggregateTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeAccountInfo(Exchange exchange, User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
    }
}