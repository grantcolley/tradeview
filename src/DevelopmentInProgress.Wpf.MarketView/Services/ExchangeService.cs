using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.Wpf.MarketView.Extensions;
using DevelopmentInProgress.Wpf.MarketView.Model;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.MarketView.Services
{
    public class ExchangeService : IExchangeService
    {
        private IExchangeApi exchangeApi;

        public ExchangeService(IExchangeApi exchangeApi)
        {
            this.exchangeApi = exchangeApi;
        }

        public async Task<Interface.Order> PlaceOrder(Interface.User user, Interface.ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await exchangeApi.PlaceOrder(user, clientOrder, recWindow, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<string> CancelOrderAsync(Interface.User user, string symbol, long orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await exchangeApi.CancelOrderAsync(user, symbol, orderId, newClientOrderId, recWindow, cancellationToken).ConfigureAwait(false);
            return result;
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
            exchangeApi.SubscribeStatistics(e =>
            {
                var stats = e.Statistics.ToList();

                (from sy in symbols join st in stats on sy.Name equals st.Symbol select sy.UpdateStatistics(st)).ToList();

            }, exception, cancellationToken);
        }

        public async Task<IEnumerable<Interface.AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var aggregateTrades = await exchangeApi.GetAggregateTradesAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            return aggregateTrades;
        }

        public async Task<Interface.OrderBook> GetOrderBookAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var orderBook = await exchangeApi.GetOrderBookAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            return orderBook;
        }

        public async Task<IEnumerable<Order>> GetOpenOrdersAsync(Interface.User user, string symbol = null, long recWindow = 0, Action<Exception> exception = default(Action<Exception>), CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await exchangeApi.GetOpenOrdersAsync(user, symbol, recWindow, exception, cancellationToken).ConfigureAwait(false);
            var orders = result.Select(o => o.GetViewOrder()).ToList();
            return orders;
        }

        public void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeOrderBook(symbol, limit, callback, exception, cancellationToken);
        }

        public async Task<Account> GetAccountInfoAsync(string apiKey, string apiSecret, CancellationToken cancellationToken)
        {
            var accountInfo = await exchangeApi.GetAccountInfoAsync(new Interface.User { ApiKey = apiKey, ApiSecret = apiSecret}, cancellationToken).ConfigureAwait(false);
            return new Account(accountInfo);
        }

        public void SubscribeAggregateTrades(string symbol, int limit, Action<AggregateTradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeAggregateTrades(symbol, limit, callback, exception, cancellationToken);
        }

        public void SubscribeAccountInfo(Interface.User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeAccountInfo(user, callback, exception, cancellationToken);
        }

        private async Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken)
        {
            var results = await exchangeApi.GetSymbolsAsync(cancellationToken).ConfigureAwait(false);
            var symbols = results.Select(s => s.GetViewSymbol()).ToList();
            return symbols;
        }

        private async Task<IEnumerable<SymbolStatistics>> Get24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            var results = await exchangeApi.Get24HourStatisticsAsync(cancellationToken).ConfigureAwait(false);
            var symbols = results.Select(s => s.GetViewSymbolStatistics()).ToList();
            return symbols;
        }
    }
}