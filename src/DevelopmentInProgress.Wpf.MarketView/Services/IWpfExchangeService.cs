using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.MarketView.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.MarketView.Services
{
    public interface IWpfExchangeService
    {
        Task<Interface.Order> PlaceOrder(Interface.User user, Interface.ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> CancelOrderAsync(Interface.User user, string symbol, long orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken));
        Task<Account> GetAccountInfoAsync(string apiKey, string apiSecret, CancellationToken cancellationToken);
        Task<IEnumerable<Symbol>> GetSymbols24HourStatisticsAsync(CancellationToken cancellationToken);
        Task<Interface.OrderBook> GetOrderBookAsync(string symbol, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<Interface.AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<Order>> GetOpenOrdersAsync(Interface.User user, string symbol = null, long recWindow = 0, Action<Exception> exception = default(Action<Exception>), CancellationToken cancellationToken = default(CancellationToken));
        void SubscribeStatistics(IEnumerable<Symbol> symbols, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeAggregateTrades(string symbol, int limit, Action<AggregateTradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeAccountInfo(Interface.User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
    }
}