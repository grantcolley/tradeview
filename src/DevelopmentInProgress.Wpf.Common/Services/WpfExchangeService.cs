using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.Wpf.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Model;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.Common.Services
{
    public class WpfExchangeService : IWpfExchangeService
    {
        private IExchangeService exchangeService;

        public WpfExchangeService(IExchangeService exchangeService)
        {
            this.exchangeService = exchangeService;
        }

        public async Task<IEnumerable<Symbol>> GetSymbols24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            var symbols = await GetSymbolsAsync(cancellationToken).ConfigureAwait(false);
            var stats = await Get24HourStatisticsAsync(cancellationToken).ConfigureAwait(false);

            var updatedSymbols = (from sy in symbols
                                 join st in stats on sy.Name equals st.Symbol
                                 select sy.JoinStatistics(st)).ToList();

            return updatedSymbols;
        }

        public void SubscribeStatistics(IEnumerable<Symbol> symbols, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeService.SubscribeStatistics(e =>
            {
                var stats = e.Statistics.ToList();

                (from sy in symbols join st in stats on sy.Name equals st.Symbol select sy.UpdateStatistics(st)).ToList();

            }, exception, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetOpenOrdersAsync(Interface.User user, string symbol = null, long recWindow = 0, Action<Exception> exception = default(Action<Exception>), CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await exchangeService.GetOpenOrdersAsync(user, symbol, recWindow, exception, cancellationToken).ConfigureAwait(false);
            var orders = result.Select(o => o.GetViewOrder()).ToList();
            return orders;
        }

        public async Task<Account> GetAccountInfoAsync(string apiKey, string apiSecret, CancellationToken cancellationToken)
        {
            var accountInfo = await exchangeService.GetAccountInfoAsync(new Interface.User { ApiKey = apiKey, ApiSecret = apiSecret}, cancellationToken).ConfigureAwait(false);
            return new Account(accountInfo);
        }

        private async Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken)
        {
            var results = await exchangeService.GetSymbolsAsync(cancellationToken).ConfigureAwait(false);
            var symbols = results.Select(s => s.GetViewSymbol()).ToList();
            return symbols;
        }

        private async Task<IEnumerable<SymbolStatistics>> Get24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            var results = await exchangeService.Get24HourStatisticsAsync(cancellationToken).ConfigureAwait(false);
            var symbols = results.Select(s => s.GetViewSymbolStatistics()).ToList();
            return symbols;
        }
        
        public async Task<Interface.Order> PlaceOrder(Interface.User user, Interface.ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await exchangeService.PlaceOrder(user, clientOrder, recWindow, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<string> CancelOrderAsync(Interface.User user, string symbol, long orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await exchangeService.CancelOrderAsync(user, symbol, orderId, newClientOrderId, recWindow, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<IEnumerable<Interface.AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var aggregateTrades = await exchangeService.GetAggregateTradesAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            return aggregateTrades;
        }

        public async Task<IEnumerable<Interface.Trade>> GetTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var trades = await exchangeService.GetTradesAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            return trades;
        }

        public async Task<Interface.OrderBook> GetOrderBookAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var orderBook = await exchangeService.GetOrderBookAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            return orderBook;
        }

        public async Task<IEnumerable<Interface.AccountTrade>> GetAccountTradesAsync(Interface.User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var accountTrades = await exchangeService.GetAccountTradesAsync(user, symbol, startDate, endDate, recWindow, cancellationToken);
            return accountTrades;
        }

        public void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeService.SubscribeOrderBook(symbol, limit, callback, exception, cancellationToken);
        }

        public void SubscribeAggregateTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeService.SubscribeAggregateTrades(symbol, limit, callback, exception, cancellationToken);
        }

        public void SubscribeTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeService.SubscribeTrades(symbol, limit, callback, exception, cancellationToken);
        }

        public void SubscribeAccountInfo(Interface.User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeService.SubscribeAccountInfo(user, callback, exception, cancellationToken);
        }
    }
}