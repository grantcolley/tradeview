using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Model;
using DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers.Data;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestBinanceExchangeApi : IExchangeApi
    {
        public bool AggregateTradesException { get; set; }
        public bool OrderBookException { get; set; }
        public bool StatisticsException { get; set; }
        public bool AccountInfoException { get; set; }

        public string NameDelimiter => throw new NotImplementedException();

        public Task<string> CancelOrderAsync(User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Symbol>> GetSymbols24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SymbolStats>> Get24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<AccountInfo> GetAccountInfoAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Candlestick>> GetCandlesticksAsync(string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = 0, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetOpenOrdersAsync(User user, string symbol = null, long recWindow = 0, Action<Exception> exception = null, CancellationToken cancellationToken = default)
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

        public Task<IEnumerable<Trade>> GetTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Order> PlaceOrder(User user, ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task SubscribeAccountInfo(User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                callback.Invoke(new AccountInfoEventArgs { AccountInfo = TestDataHelper.AccountInfo });
                await Task.Delay(500);

                if (AccountInfoException)
                {
                    exception.Invoke(new Exception("SubscribeAccountInfo"));
                }
            }
        }

        public async Task SubscribeAggregateTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var localSymbol = symbol;
            while (!cancellationToken.IsCancellationRequested)
            {
                callback.Invoke(new
                    TradeEventArgs
                { Trades = TestDataHelper.GetAggregateTradesUpdated(localSymbol) });
                await Task.Delay(500);


                if (AggregateTradesException)
                {
                    exception.Invoke(new Exception("SubscribeAggregateTrades"));
                }
            }
        }

        public Task SubscribeCandlesticks(string symbol, CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var localSymbol = symbol;
            while (!cancellationToken.IsCancellationRequested)
            {
                callback.Invoke(new OrderBookEventArgs { OrderBook = TestDataHelper.GetOrderBook(localSymbol) });
                await Task.Delay(500);

                if (OrderBookException)
                {
                    exception.Invoke(new Exception("SubscribeOrderBook"));
                }
            }
        }

        public async Task SubscribeStatistics(Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                callback.Invoke(new StatisticsEventArgs { Statistics = TestDataHelper.SymbolsStatistics });
                await Task.Delay(500);

                if (StatisticsException)
                {
                    exception.Invoke(new Exception("SubscribeStatistics"));
                }
            }
        }

        public Task SubscribeTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeStatistics(IEnumerable<string> symbols, Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
