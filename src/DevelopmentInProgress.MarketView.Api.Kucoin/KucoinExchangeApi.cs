using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Model;
using Kucoin.Net;
using Kucoin.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Api.Kucoin
{
    public class KucoinExchangeApi : IExchangeApi
    {
        public async Task<string> CancelOrderAsync(User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var options = new KucoinClientOptions
            {
                ApiCredentials = new KucoinApiCredentials(user.ApiKey, user.ApiSecret, user.ApiPassPhrase)
            };

            var kucoinClient = new KucoinClient(options);
            var result = await kucoinClient.CancelOrderAsync(orderId).ConfigureAwait(false);
            return result.Data.CancelledOrderIds.First();
        }

        public async Task<AccountInfo> GetAccountInfoAsync(User user, CancellationToken cancellationToken)
        {
            var options = new KucoinClientOptions
            {
                ApiCredentials = new KucoinApiCredentials(user.ApiKey, user.ApiSecret, user.ApiPassPhrase)
            };

            var accountInfo = new AccountInfo { User = user, Balances = new List<AccountBalance>() };
            var kucoinClient = new KucoinClient(options);
            var accounts = await kucoinClient.GetAccountsAsync(accountType: KucoinAccountType.Trade).ConfigureAwait(false);
            foreach (var balance in accounts.Data)
            {
                accountInfo.Balances.Add(new AccountBalance { Asset = balance.Currency, Free = balance.Available, Locked = balance.Holds });
            }

            return accountInfo;
        }

        public Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public  Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Candlestick>> GetCandlesticksAsync(string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = 0, CancellationToken token = default(CancellationToken))
        {
            var candlestickInterval = interval.ToKucoinCandlestickInterval();
            var kucoinClient = new KucoinClient();
            var result = await kucoinClient.GetKlinesAsync(symbol, candlestickInterval, startTime, endTime);

            Func<KucoinKline, Candlestick> f = k =>
            {
                return new Candlestick
                {
                    Symbol = symbol,
                    Interval = interval,
                    OpenTime = k.StartTime,
                    Open = k.Open,
                    High = k.High,
                    Low = k.Low,
                    Close = k.Close,
                    Volume = k.Volume                      
                };
            };

            var candlesticks = (from k in result.Data select f(k)).ToList();

            return candlesticks;
        }

        public async Task<IEnumerable<Order>> GetOpenOrdersAsync(User user, string symbol = null, long recWindow = 0, Action<Exception> exception = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var options = new KucoinClientOptions
            {
                ApiCredentials = new KucoinApiCredentials(user.ApiKey, user.ApiSecret, user.ApiPassPhrase)
            };

            var kucoinClient = new KucoinClient(options);
            var results = await kucoinClient.GetOrdersAsync(null, null, null, null, null, KucoinOrderStatus.Active).ConfigureAwait(false);

            var orders = (from o in results.Data.Items
                          select new Order
                          {
                              User = user,
                              Symbol = o.Symbol,
                              Id = o.Id,
                              ClientOrderId = o.ClientOrderId,
                              Price = o.Price,
                              OriginalQuantity = o.Quantity,
                              TimeInForce = o.TimeInForce.ToMarketViewTimeInForce(),
                              Type = o.Type.ToMarketViewOrderType(),
                              Side = o.Side.ToMarketViewOrderSide(),
                              StopPrice = o.StopPrice,
                              IcebergQuantity = o.VisibleIcebergSize,
                              Time = o.CreatedAt,
                              //Fills = o.Fills?.Select(f => new Interface.Model.Fill
                              //{
                              //    Price = f.Price,
                              //    Quantity = f.Quantity,
                              //    Commission = f.Commission,
                              //    CommissionAsset = f.CommissionAsset,
                              //    TradeId = f.TradeId
                              //})
                          }).ToList();

            return orders;
        }

        public async Task<OrderBook> GetOrderBookAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var kucoinClient = new KucoinClient();
            var result = await kucoinClient.GetAggregatedPartialOrderBookAsync(symbol, limit).ConfigureAwait(false);

            var orderBook = new OrderBook
            {
                Symbol = symbol,
                FirstUpdateId = result.Data.Sequence,
                LastUpdateId = result.Data.Sequence
            };

            orderBook.Asks = (from ask in result.Data.Asks select new OrderBookPriceLevel { Price = ask.Price, Quantity = ask.Quantity }).ToList();
            orderBook.Bids = (from bid in result.Data.Bids select new OrderBookPriceLevel { Price = bid.Price, Quantity = bid.Quantity }).ToList();

            return orderBook;
        }

        public async Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken)
        {
            var kucoinClient = new KucoinClient();
            var result = await kucoinClient.GetSymbolsAsync().ConfigureAwait(false);
            var symbols = result.Data.Select(s => new Symbol
            {
                ExchangeSymbol = s.Symbol,
                NotionalMinimumValue = s.QuoteMinSize,
                BaseAsset = new Asset { Symbol = s.BaseCurrency },
                QuoteAsset = new Asset { Symbol = s.QuoteCurrency },
                Price = new InclusiveRange { Increment = s.PriceIncrement, Maximum = s.QuoteMaxSize, Minimum = s.PriceIncrement },
                Quantity = new InclusiveRange { Increment = s.BaseIncrement, Maximum = s.BaseMaxSize, Minimum = s.BaseIncrement },
                SymbolStatistics = new SymbolStats { Symbol = $"{s.BaseCurrency}{s.QuoteCurrency}" },
                OrderTypes = new[] { OrderType.Limit, OrderType.Market, OrderType.StopLoss, OrderType.StopLossLimit, OrderType.TakeProfit, OrderType.TakeProfitLimit }
            }).ToList();

            var currencies = await kucoinClient.GetCurrenciesAsync().ConfigureAwait(false);

            Func<Asset, KucoinCurrency, Asset> f = (a, c) => 
            {
                a.Precision = c.Precision;
                return a;
            };

            (from s in symbols
             join c in currencies.Data on s.BaseAsset.Symbol equals c.Currency
             select f(s.BaseAsset, c)).ToList();

            (from s in symbols
             join c in currencies.Data on s.QuoteAsset.Symbol equals c.Currency
             select f(s.QuoteAsset, c)).ToList();

            return symbols;
        }

        public async Task<IEnumerable<Symbol>> GetSymbols24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            return await GetSymbolsAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<SymbolStats>> Get24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Trade>> GetTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var kucoinClint = new KucoinClient();
            var result = await kucoinClint.GetTradeHistoryAsync(symbol).ConfigureAwait(false);
            var trades = result.Data.Select(t => new Trade
            {
                Symbol = symbol,
                Id = t.Sequence,
                Price = t.Price,
                Quantity = t.Quantity,
                Time = t.Timestamp,
                IsBuyerMaker = t.Side == KucoinOrderSide.Sell ? true : false
            }).ToList();

            return trades;
        }

        public async Task<Order> PlaceOrder(User user, ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var options = new KucoinClientOptions
            {
                ApiCredentials = new KucoinApiCredentials(user.ApiKey, user.ApiSecret, user.ApiPassPhrase)
            };

            var kucoinClient = new KucoinClient(options);
            var placeOrderResult = await kucoinClient.PlaceOrderAsync(
                clientOrder.Symbol,
                clientOrder.Side.ToKucoinOrderSide(),
                clientOrder.Type.ToKucoinNewOrderType(),
                clientOrder.Price,
                clientOrder.Quantity,
                null,
                clientOrder.TimeInForce.ToKucoinTimeInForce(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                clientOrder.StopPrice,
                null,
                null
                ).ConfigureAwait(false);

            if(!placeOrderResult.Success)
            {
                throw new Exception($"Error Code : {placeOrderResult.Error.Code} Message : {placeOrderResult.Error.Message}");
            }

            var orderResult = await kucoinClient.GetOrderAsync(placeOrderResult.Data.OrderId).ConfigureAwait(false);

            if (orderResult.Success)
            {
                var order = new Order
                {
                    User = user,
                    Symbol = orderResult.Data.Symbol,
                    //Id = orderResult.Data.Id,
                    ClientOrderId = orderResult.Data.ClientOrderId,
                    Price = orderResult.Data.Price,
                    OriginalQuantity = orderResult.Data.Quantity,
                    TimeInForce = orderResult.Data.TimeInForce.ToMarketViewTimeInForce(),
                    Type = orderResult.Data.Type.ToMarketViewOrderType(),
                    Side = orderResult.Data.Side.ToMarketViewOrderSide(),
                    StopPrice = orderResult.Data.StopPrice,
                    IcebergQuantity = orderResult.Data.VisibleIcebergSize,
                    Time = orderResult.Data.CreatedAt
                };

                return order;
            }
            else
            {
                throw new Exception($"Error Code : {orderResult.Error.Code} Message : {orderResult.Error.Message}");
            }
        }

        public void SubscribeAccountInfo(User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var localUser = user;

            var kucoinClient = new KucoinSocketClient(new KucoinSocketClientOptions { ApiCredentials = new KucoinApiCredentials(user.ApiKey, user.ApiSecret, user.ApiPassPhrase) });

            CallResult<UpdateSubscription> result = null;

            try
            {
                result = kucoinClient.SubscribeToBalanceChanges(data =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        kucoinClient.Unsubscribe(result.Data).FireAndForget();
                        return;
                    }

                    try
                    {
                        var accountInfo = GetAccountInfoAsync(localUser, cancellationToken).GetAwaiter().GetResult();

                        callback.Invoke(new AccountInfoEventArgs { AccountInfo = accountInfo });
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

        public void SubscribeAggregateTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
                                Symbol = data.Symbol.Replace("-", string.Empty),
                                CloseTime = data.Timestamp,
                                Volume = data.Volume,
                                LowPrice = data.Low,
                                HighPrice = data.High,
                                LastPrice = data.LastPrice,
                                PriceChange = data.ChangePrice,
                                PriceChangePercent = data.ChangePercentage * 100
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
                            Symbol = data.Symbol.Replace("-", string.Empty),
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