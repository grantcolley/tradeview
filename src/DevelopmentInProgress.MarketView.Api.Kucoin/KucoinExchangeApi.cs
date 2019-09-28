using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Model;
using Kucoin.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Api.Kucoin
{
    public class KucoinExchangeApi : IExchangeApi
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
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Candlestick>> GetCandlesticksAsync(string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = 0, CancellationToken token = default(CancellationToken))
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

        public Task<IEnumerable<Trade>> GetTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Order> PlaceOrder(User user, ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void SubscribeAccountInfo(User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAggregateTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            SubscribeTrades(symbol, limit, callback, exception, cancellationToken);
        }

        public void SubscribeCandlesticks(string symbol, CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var kucoinClient = new KucoinSocketClient();

            CallResult<UpdateSubscription> result = null;

            try
            {
                result = kucoinClient.SubscribeToAggregatedOrderBookUpdates(symbol, data =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        kucoinClient.Unsubscribe(result.Data).FireAndForget();
                        return;
                    }

                    try
                    {
                        var orderBook = new OrderBook
                        {
                            Symbol = data.Symbol,
                            FirstUpdateId = data.SequenceStart,
                            LastUpdateId = data.SequenceEnd
                        };

                        orderBook.Asks = (from ask in data.Changes.Asks select new OrderBookPriceLevel { Id = ask.Sequence, Price = ask.Price, Quantity = ask.Quantity}).ToList();
                        orderBook.Bids = (from bid in data.Changes.Bids select new OrderBookPriceLevel { Id = bid.Sequence, Price = bid.Price, Quantity = bid.Quantity }).ToList();

                        callback.Invoke(new OrderBookEventArgs { OrderBook = orderBook });
                    }
                    catch (Exception ex)
                    {
                        kucoinClient.Unsubscribe(result.Data).FireAndForget();
                        exception.Invoke(ex);
                        return;
                    }
                });
            }
            catch (Exception ex)
            {
                if (result != null)
                {
                    kucoinClient.Unsubscribe(result.Data).FireAndForget();
                }

                exception.Invoke(ex);
            }
        }

        public void SubscribeStatistics(Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var kucoinSocketClient = new KucoinSocketClient();

            CallResult<UpdateSubscription> result = null;

            try
            {
                var kucoinClient = new KucoinClient();
                var markets = kucoinClient.GetMarkets();

                foreach (var market in markets.Data)
                {
                    result = kucoinSocketClient.SubscribeToSnapshotUpdates(market, data =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            kucoinSocketClient.Unsubscribe(result.Data).FireAndForget();
                            return;
                        }

                        try
                        {
                            var symbolStats = new SymbolStats
                            {
                                Symbol = data.Symbol,
                                CloseTime = data.Timestamp,
                                Volume = data.Volume,
                                LowPrice = data.Low,
                                HighPrice = data.High,
                                LastPrice = data.LastPrice,
                                PriceChange = data.ChangePrice,
                                PriceChangePercent = data.ChangePercentage
                            };

                            callback.Invoke(new StatisticsEventArgs { Statistics = new[] { symbolStats } });
                        }
                        catch (Exception ex)
                        {
                            kucoinSocketClient.Unsubscribe(result.Data).FireAndForget();
                            exception.Invoke(ex);
                            return;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                if (result != null)
                {
                    kucoinSocketClient.Unsubscribe(result.Data).FireAndForget();
                }

                exception.Invoke(ex);
            }
        }

        public void SubscribeTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var kucoinClient = new KucoinSocketClient();

            CallResult<UpdateSubscription> result = null;

            try
            {
                result = kucoinClient.SubscribeToTradeUpdates(symbol, data =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        kucoinClient.Unsubscribe(result.Data).FireAndForget();
                        return;
                    }

                    try
                    {
                        var trade = new Trade
                        {
                            Id = data.Sequence,
                            Symbol = data.Symbol,
                            Price = data.Price,
                            Time = data.Timestamp,
                            Quantity = data.Quantity
                        };

                        callback.Invoke(new TradeEventArgs { Trades = new[] { trade } });
                    }
                    catch (Exception ex)
                    {
                        kucoinClient.Unsubscribe(result.Data).FireAndForget();
                        exception.Invoke(ex);
                        return;
                    }
                });
            }
            catch (Exception ex)
            {
                if (result != null)
                {
                    kucoinClient.Unsubscribe(result.Data).FireAndForget();
                }

                exception.Invoke(ex);
            }
        }
    }
}