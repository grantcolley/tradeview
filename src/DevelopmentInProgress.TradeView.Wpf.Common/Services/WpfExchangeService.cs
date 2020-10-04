using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public class WpfExchangeService : IWpfExchangeService
    {
        private readonly IExchangeService exchangeService;

        public WpfExchangeService(IExchangeService exchangeService)
        {
            this.exchangeService = exchangeService;
        }

        public string GetNameDelimiter(Exchange exchange)
        {
            return exchangeService.GetNameDelimiter(exchange);
        }

        public async Task<IEnumerable<Order>> GetOpenOrdersAsync(Exchange exchange, Core.Model.User user, string symbol = null, long recWindow = 0, Action<Exception> exception = default, CancellationToken cancellationToken = default)
        {
            var result = await exchangeService.GetOpenOrdersAsync(exchange, user, symbol, recWindow, exception, cancellationToken).ConfigureAwait(false);
            var orders = result.Select(o => o.GetViewOrder()).ToList();
            return orders;
        }

        public async Task<Account> GetAccountInfoAsync(Exchange exchange, Core.Model.User user, CancellationToken cancellationToken)
        {
            var accountInfo = await exchangeService.GetAccountInfoAsync(exchange, user, cancellationToken).ConfigureAwait(false);
            return new Account(accountInfo);
        }

        public async Task<IEnumerable<Symbol>> GetSymbols24HourStatisticsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            var results = await exchangeService.GetSymbols24HourStatisticsAsync(exchange, cancellationToken).ConfigureAwait(false);
            var symbols = results.Select(s => s.GetViewSymbol()).ToList();
            return symbols;
        }

        public async Task<IEnumerable<Symbol>> GetSymbolsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            var results = await exchangeService.GetSymbolsAsync(exchange, cancellationToken).ConfigureAwait(false);
            var symbols = results.Select(s => s.GetViewSymbol()).ToList();
            return symbols;
        }

        public Task<Core.Model.Order> PlaceOrder(Exchange exchange, Core.Model.User user, Core.Model.ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            return exchangeService.PlaceOrder(exchange, user, clientOrder, recWindow, cancellationToken);
        }

        public Task<string> CancelOrderAsync(Exchange exchange, Core.Model.User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            return exchangeService.CancelOrderAsync(exchange, user, symbol, orderId, newClientOrderId, recWindow, cancellationToken);
        }

        public Task<IEnumerable<Core.Model.AggregateTrade>> GetAggregateTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            return exchangeService.GetAggregateTradesAsync(exchange, symbol, limit, cancellationToken);
        }

        public Task<IEnumerable<Core.Model.Trade>> GetTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            return exchangeService.GetTradesAsync(exchange, symbol, limit, cancellationToken);
        }

        public Task<Core.Model.OrderBook> GetOrderBookAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            return exchangeService.GetOrderBookAsync(exchange, symbol, limit, cancellationToken);
        }

        public Task<IEnumerable<Core.Model.AccountTrade>> GetAccountTradesAsync(Exchange exchange, Core.Model.User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            return exchangeService.GetAccountTradesAsync(exchange, user, symbol, startDate, endDate, recWindow, cancellationToken);
        }

        public async Task<IEnumerable<Candlestick>> GetCandlesticksAsync(Exchange exchange, string symbol, Core.Model.CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = default, CancellationToken token = default)
        {
            var results = await exchangeService.GetCandlesticksAsync(exchange, symbol, interval, startTime, endTime, limit, token).ConfigureAwait(false);
            var candlesticks = results.Select(c => c.ToViewCandlestick()).ToList();
            return candlesticks;
        }

        public async Task SubscribeStatistics(Exchange exchange, IEnumerable<Symbol> symbols, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var names = symbols.Select(s => s.ExchangeSymbol).ToList();

            await exchangeService.SubscribeStatistics(exchange, names, e =>
            {
                var stats = e.Statistics.ToList();

                (from sy in symbols join st in stats on sy.ExchangeSymbol equals st.Symbol select sy.UpdateStatistics(st)).ToList();

            }, exception, cancellationToken).ConfigureAwait(false);
        }

        public Task SubscribeCandlesticks(Exchange exchange, string symbol, Core.Model.CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return exchangeService.SubscribeCandlesticks(exchange, symbol, candlestickInterval, limit, callback, exception, cancellationToken);
        }

        public Task SubscribeOrderBook(Exchange exchange, string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return exchangeService.SubscribeOrderBook(exchange, symbol, limit, callback, exception, cancellationToken);
        }

        public Task SubscribeAggregateTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return exchangeService.SubscribeAggregateTrades(exchange, symbol, limit, callback, exception, cancellationToken);
        }

        public Task SubscribeTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return exchangeService.SubscribeTrades(exchange, symbol, limit, callback, exception, cancellationToken);
        }

        public Task SubscribeAccountInfo(Exchange exchange, Core.Model.User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return exchangeService.SubscribeAccountInfo(exchange, user, callback, exception, cancellationToken);
        }
    }
}