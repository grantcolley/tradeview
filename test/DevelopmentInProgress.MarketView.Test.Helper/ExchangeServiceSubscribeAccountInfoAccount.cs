using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Test.Helper
{
    public class ExchangeServiceSubscribeAccountInfoAccount : IExchangeService
    {
        public Task<string> CancelOrderAsync(User user, string symbol, long orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SymbolStats>> Get24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<AccountInfo> GetAccountInfoAsync(User user, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<AccountInfo>();
            tcs.SetResult(TestHelper.AccountInfo);
            return tcs.Task;
        }

        public Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Trade>> GetTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetOpenOrdersAsync(User user, string symbol = null, long recWindow = 0, Action<Exception> exception = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<OrderBook> GetOrderBookAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Order> PlaceOrder(User user, ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Candlestick>> GetCandlesticksAsync(string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = default(int), CancellationToken token = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void SubscribeAccountInfo(User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var accountInfo = TestHelper.AccountInfo;
            var btc = accountInfo.Balances.Single(ab => ab.Asset.Equals("BTC"));
            var bcpt = accountInfo.Balances.Single(ab => ab.Asset.Equals("BCPT"));
            accountInfo.Balances.Remove(btc);
            accountInfo.Balances.Remove(bcpt);
            accountInfo.Balances.Add(new AccountBalance { Asset = "TEST", Free = 1, Locked = 2 });

            callback.Invoke(new AccountInfoEventArgs { AccountInfo = accountInfo });
        }

        public void SubscribeAggregateTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SubscribeTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SubscribeStatistics(Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void SubscribeCandlesticks(string symbol, CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
