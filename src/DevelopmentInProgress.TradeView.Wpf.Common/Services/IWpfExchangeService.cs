using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public interface IWpfExchangeService
    {
        string GetNameDelimiter(Exchange exchange);
        Task<Core.Model.Order> PlaceOrder(Exchange exchange, Core.Model.User user, Core.Model.ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default);
        Task<string> CancelOrderAsync(Exchange exchange, Core.Model.User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default);
        Task<Account> GetAccountInfoAsync(Exchange exchange, Core.Model.User user, CancellationToken cancellationToken);
        Task<IEnumerable<Core.Model.AccountTrade>> GetAccountTradesAsync(Exchange exchange, Core.Model.User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default);
        Task<IEnumerable<Symbol>> GetSymbols24HourStatisticsAsync(Exchange exchange, CancellationToken cancellationToken);
        Task<IEnumerable<Symbol>> GetSymbolsAsync(Exchange exchange, CancellationToken cancellationToken);
        Task<Core.Model.OrderBook> GetOrderBookAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<Core.Model.AggregateTrade>> GetAggregateTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<Core.Model.Trade>> GetTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<Order>> GetOpenOrdersAsync(Exchange exchange, Core.Model.User user, string symbol = null, long recWindow = 0, Action<Exception> exception = default, CancellationToken cancellationToken = default);
        Task<IEnumerable<Candlestick>> GetCandlesticksAsync(Exchange exchange, string symbol, Core.Model.CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = default, CancellationToken token = default);
        Task SubscribeStatistics(Exchange exchange, IEnumerable<Symbol> symbols, Action<Exception> exception, CancellationToken cancellationToken);
        Task SubscribeCandlesticks(Exchange exchange, string symbol, Core.Model.CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        Task SubscribeOrderBook(Exchange exchange, string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        Task SubscribeAggregateTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        Task SubscribeTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        Task SubscribeAccountInfo(Exchange exchange, Core.Model.User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
    }
}