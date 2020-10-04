using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Model;
using DevelopmentInProgress.TradeView.Core.Enums;

namespace DevelopmentInProgress.TradeView.Test.Helper
{
    public class ExchangeServicePlaceOrderException : IExchangeService
    {
        public string GetNameDelimiter(Exchange exchange)
        {
            throw new NotImplementedException();
        }

        public Task<string> CancelOrderAsync(Exchange exchange, User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SymbolStats>> Get24HourStatisticsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<IEnumerable<SymbolStats>>();
            tcs.SetResult(TestHelper.SymbolsStatistics);
            return tcs.Task;
        }

        public Task<AccountInfo> GetAccountInfoAsync(Exchange exchange, User user, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<AccountInfo>();
            tcs.SetResult(TestHelper.AccountInfo);
            return tcs.Task;
        }

        public Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Trade>> GetTradesAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetOpenOrdersAsync(Exchange exchange, User user, string symbol = null, long recWindow = 0, Action<Exception> exception = null, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<IEnumerable<Order>>();
            tcs.SetResult(TestHelper.Orders);
            return tcs.Task;
        }

        public Task<OrderBook> GetOrderBookAsync(Exchange exchange, string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Symbol>> GetSymbolsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<IEnumerable<Symbol>>();
            tcs.SetResult(TestHelper.Symbols);
            return tcs.Task;
        }

        public Task<Order> PlaceOrder(Exchange exchange, User user, ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            throw new Exception("failed to place order");
        }

        public Task<IEnumerable<Candlestick>> GetCandlesticksAsync(Exchange exchange, string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = default, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeAccountInfo(Exchange exchange, User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Order>();
            tcs.SetResult(null);
            return tcs.Task;
        }

        public Task SubscribeAggregateTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeTrades(Exchange exchange, string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeOrderBook(Exchange exchange, string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeStatistics(Exchange exchange, Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var ethStats = TestHelper.EthStats;
            var symbolsStats = TestHelper.SymbolsStatistics;

            var newStats = TestHelper.EthStats_UpdatedLastPrice_Upwards;

            var updatedEthStats = symbolsStats.Single(s => s.Symbol.Equals("ETHBTC"));
            updatedEthStats.PriceChange = newStats.PriceChange;
            updatedEthStats.LastPrice = newStats.LastPrice;
            updatedEthStats.PriceChangePercent = newStats.PriceChangePercent;

            callback.Invoke(new StatisticsEventArgs { Statistics = symbolsStats });

            var tcs = new TaskCompletionSource<Order>();
            tcs.SetResult(null);
            return tcs.Task;
        }

        public Task SubscribeStatistics(Exchange exchange, IEnumerable<string> symbols, Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return SubscribeStatistics(exchange, callback, exception, cancellationToken);
        }

        public Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(Exchange exchange, User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeCandlesticks(Exchange exchange, string symbol, CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Symbol>> GetSymbols24HourStatisticsAsync(Exchange exchange, CancellationToken cancellationToken)
        {
            var symbols = await GetSymbolsAsync(exchange, cancellationToken).ConfigureAwait(false);
            var symbolStatistics = await Get24HourStatisticsAsync(exchange, cancellationToken).ConfigureAwait(false);

            static Symbol f(Symbol s, SymbolStats ss)
            {
                s.SymbolStatistics = ss;
                return s;
            }

            var updatedSymbols = (from s in symbols
                                  join ss in symbolStatistics on $"{s.BaseAsset.Symbol}{s.QuoteAsset.Symbol}" equals ss.Symbol
                                  select f(s, ss)).ToList();

            return updatedSymbols;
        }

        public IExchangeApi GetExchangeApi(Exchange exchange)
        {
            throw new NotImplementedException();
        }
    }
}
