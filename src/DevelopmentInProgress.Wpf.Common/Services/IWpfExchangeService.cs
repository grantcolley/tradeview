using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Enums;
using DevelopmentInProgress.Wpf.Common.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.Common.Services
{
    public interface IWpfExchangeService
    {
        Task<Interface.Order> PlaceOrder(Exchange exchange, Interface.User user, Interface.ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> CancelOrderAsync(Exchange exchange, Interface.User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken));
        Task<Account> GetAccountInfoAsync(Exchange exchange, Interface.User user, CancellationToken cancellationToken);
        Task<IEnumerable<Interface.AccountTrade>> GetAccountTradesAsync(Exchange exchange, Interface.User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Symbol>> GetSymbols24HourStatisticsAsync(Exchange exchange, CancellationToken cancellationToken);
        Task<IEnumerable<Symbol>> GetSymbolsAsync(Exchange exchange, CancellationToken cancellationToken);
        Task<Interface.OrderBook> GetOrderBookAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<Interface.AggregateTrade>> GetAggregateTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<Interface.Trade>> GetTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<Order>> GetOpenOrdersAsync(Exchange exchange, Interface.User user, string symbol = null, long recWindow = 0, Action<Exception> exception = default(Action<Exception>), CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Candlestick>> GetCandlesticksAsync(Exchange exchange, string symbol, Interface.CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = default(int), CancellationToken token = default(CancellationToken));
        void SubscribeStatistics(Exchange exchange, IEnumerable<Symbol> symbols, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeCandlesticks(Exchange exchange, string symbol, Interface.CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeOrderBook(Exchange exchange, string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeAggregateTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeAccountInfo(Exchange exchange, Interface.User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
    }
}