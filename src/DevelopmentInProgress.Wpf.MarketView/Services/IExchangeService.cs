using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.MarketView.Services
{
    public interface IExchangeService
    {
        Task<Account> GetAccountInfoAsync(string apiKey, string apiSecret, CancellationToken cancellationToken);
        Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<SymbolStatistics>> Get24HourStatisticsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Symbol>> GetSymbols24HourStatisticsAsync(CancellationToken cancellationToken);
        Task<Interface.OrderBook> GetOrderBookAsync(string symbol, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<Interface.AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken);
        void SubscribeStatistics(IEnumerable<Symbol> symbols, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        void SubscribeAggregateTrades(string symbol, int limit, Action<AggregateTradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
    }
}