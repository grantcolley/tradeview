using Binance.Net.Clients;
using Binance.Net.Interfaces;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects.Models.Spot;
using Binance.Net.Objects.Options;
using Binance.Net.SymbolOrderBooks;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Model;
using System.Data;
using System.Globalization;

namespace DevelopmentInProgress.TradeView.Api.BinanceNet
{
    public class BinanceExchangeApi : IExchangeApi
    {
        public string NameDelimiter { get; } = "-";

        public async Task<string> CancelOrderAsync(User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using var binanceRestClient = GetBinanceRestClient(user);
            var result = await binanceRestClient.SpotApi.Trading.CancelOrderAsync(symbol, Convert.ToInt64(orderId, CultureInfo.InvariantCulture), ct:cancellationToken).ConfigureAwait(false);
            return result.Data.ClientOrderId;
        }

        public async Task<AccountInfo> GetAccountInfoAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var accountInfo = new AccountInfo
            {
                User = user,
                Exchange = Exchange.Binance
            };

            using var binanceRestClient = GetBinanceRestClient(user);
            var accounts = await binanceRestClient.SpotApi.Account.GetBalancesAsync(ct: cancellationToken).ConfigureAwait(false);
            foreach (var balance in accounts.Data)
            {
                accountInfo.Balances.Add(new AccountBalance { Asset = balance.Asset, Free = balance.Available, Locked = balance.Locked });
            }

            return accountInfo;
        }

        public Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Candlestick>> GetCandlesticksAsync(string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = 0, CancellationToken token = default)
        {
            var candlestickInterval = interval.ToKlineInterval();

            using var binanceRestClient = new BinanceRestClient();
            var result = await binanceRestClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, candlestickInterval, startTime, endTime, ct: token).ConfigureAwait(false);

            Candlestick f(IBinanceKline k)
            {
                return new Candlestick
                {
                    Symbol = symbol,
                    Exchange = Exchange.Binance,
                    Interval = interval,
                    OpenTime = k.OpenTime,
                    Open = k.OpenPrice,
                    High = k.HighPrice,
                    Low = k.LowPrice,
                    Close = k.ClosePrice,
                    Volume = k.Volume
                };
            };

            var candlesticks = (from k in result.Data select f(k)).ToList();

            return candlesticks;
        }

        public async Task<IEnumerable<Order>> GetOpenOrdersAsync(User user, string symbol = null, long recWindow = 0, Action<Exception> exception = null, CancellationToken cancellationToken = default)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using var binanceRestClient = GetBinanceRestClient(user);
            var results = await binanceRestClient.SpotApi.Trading.GetOrdersAsync(symbol, receiveWindow: (int)recWindow, ct: cancellationToken).ConfigureAwait(false);

            var orders = (from o in results.Data
                          select new Order
                          {
                              User = user,
                              Symbol = o.Symbol,
                              Exchange = Exchange.Binance,
                              Id = $"{o.Id}",
                              ClientOrderId = o.ClientOrderId,
                              Price = o.Price,
                              OriginalQuantity = o.Quantity,
                              TimeInForce = o.TimeInForce.ToTradeViewTimeInForce(),
                              Type = o.Type.ToTradeViewOrderType(),
                              Side = o.Side.ToTradeViewOrderSide(),
                              StopPrice = o.StopPrice ?? throw new DataException($"Order Id: {o.Id} - StopPrice missing"),
                              IcebergQuantity = o.IcebergQuantity ?? 0,
                              Time = o.CreateTime
                          }).ToList();

            return orders;
        }

        public async Task<OrderBook> GetOrderBookAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            using var binanceRestClient = new BinanceRestClient();
            var result = await binanceRestClient.SpotApi.ExchangeData.GetOrderBookAsync(symbol, limit, ct: cancellationToken).ConfigureAwait(false);

            var orderBook = new OrderBook
            {
                Symbol = symbol,
                Exchange = Exchange.Binance,
                LastUpdateId = result.Data.LastUpdateId
            };

            orderBook.Asks = (from ask in result.Data.Asks select new OrderBookPriceLevel { Price = ask.Price, Quantity = ask.Quantity }).ToList();
            orderBook.Bids = (from bid in result.Data.Bids select new OrderBookPriceLevel { Price = bid.Price, Quantity = bid.Quantity }).ToList();

            return orderBook;
        }

        public async Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken)
        {
            using var binanceRestClient = new BinanceRestClient();

            static Symbol Symbol(BinanceSymbol s)
            {
                BinanceSymbolPriceFilter price = s.Filters.OfType<BinanceSymbolPriceFilter>().First();
                BinanceSymbolLotSizeFilter lotSize = s.Filters.OfType<BinanceSymbolLotSizeFilter>().First();
                BinanceSymbolNotionalFilter notional = s.Filters.OfType<BinanceSymbolNotionalFilter>().First();
                return new Symbol
                {
                    Name = $"{s.BaseAsset}{s.QuoteAsset}",
                    ExchangeSymbol = $"{s.BaseAsset}{s.QuoteAsset}",
                    Exchange = Exchange.Binance,
                    NotionalMinimumValue = notional.MinNotional,
                    BaseAsset = new Asset { Symbol = s.BaseAsset, Precision = s.BaseAssetPrecision },
                    Price = new InclusiveRange { Increment = price.TickSize, Minimum = price.MinPrice, Maximum = price.MaxPrice },
                    Quantity = new InclusiveRange { Increment = lotSize.StepSize, Minimum = lotSize.MinQuantity, Maximum = lotSize.MaxQuantity },
                    QuoteAsset = new Asset { Symbol = s.QuoteAsset, Precision = s.QuoteAssetPrecision },
                    IsIcebergAllowed = s.IcebergAllowed,
                    OrderTypes = new[] { OrderType.Limit, OrderType.Market, OrderType.StopLoss, OrderType.StopLossLimit, OrderType.TakeProfit, OrderType.TakeProfitLimit }
                };
            }

            var result = await binanceRestClient.SpotApi.ExchangeData.GetExchangeInfoAsync().ConfigureAwait(false);
            var symbols = result.Data.Symbols.Select(s => Symbol(s)).ToList();
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
            using var binanceRestClient = new BinanceRestClient();
            var result = await binanceRestClient.SpotApi.ExchangeData.GetRecentTradesAsync(symbol).ConfigureAwait(false);
            var trades = result.Data.Select(t => new Trade
            {
                Symbol = symbol,
                Exchange = Exchange.Binance,
                Id = t.OrderId,
                Price = t.Price,
                Quantity = t.BaseQuantity,
                Time = t.TradeTime,
                IsBuyerMaker = t.BuyerIsMaker
            }).ToList();

            return trades;
        }

        public async Task<Order> PlaceOrder(User user, ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (clientOrder == null)
            {
                throw new ArgumentNullException(nameof(clientOrder));
            }

            using var binanceRestClient = GetBinanceRestClient(user);
            var placeOrderResult = await binanceRestClient.SpotApi.Trading.PlaceOrderAsync(
                clientOrder.Symbol,
                clientOrder.Side.ToBinanceOrderSide(),
                clientOrder.Type.ToSpotOrderType(),
                clientOrder.Quantity,
                price: clientOrder.Price,
                timeInForce: clientOrder.TimeInForce.ToBinanceTimeInForce(),
                stopPrice: clientOrder.StopPrice
                 ).ConfigureAwait(false);

            if (!placeOrderResult.Success)
            {
                throw new Exception($"Error Code : {placeOrderResult?.Error?.Code} Message : {placeOrderResult?.Error?.Message}");
            }

            if (placeOrderResult.Success)
            {
                var order = new Order
                {
                    User = user,
                    Exchange = Exchange.Binance,
                    Symbol = placeOrderResult.Data.Symbol,
                    Id = Convert.ToString(placeOrderResult.Data.Id, CultureInfo.InvariantCulture),
                    ClientOrderId = placeOrderResult.Data.ClientOrderId,
                    Price = placeOrderResult.Data.Price,
                    OriginalQuantity = placeOrderResult.Data.Quantity,
                    TimeInForce = placeOrderResult.Data.TimeInForce.ToTradeViewTimeInForce(),
                    Type = placeOrderResult.Data.Type.ToTradeViewOrderType(),
                    Side = placeOrderResult.Data.Side.ToTradeViewOrderSide(),
                    StopPrice = placeOrderResult.Data.StopPrice,
                    IcebergQuantity = placeOrderResult.Data.IcebergQuantity,
                    Time = placeOrderResult.Data.CreateTime
                };

                return order;
            }
            else
            {
                throw new Exception($"Error Code : {placeOrderResult?.Error?.Code} Message : {placeOrderResult?.Error?.Message}");
            }
        }

        public async Task SubscribeAccountInfo(User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            ApiCredentials apiCredentials = new ApiCredentials(user.ApiKey, user.ApiSecret, user.ApiPassPhrase);

            BinanceSocketClient binanceSocketClient = new BinanceSocketClient();
            binanceSocketClient.SetApiCredentials(apiCredentials);

            BinanceRestClient binanceRestClient = new BinanceRestClient();
            binanceRestClient.SetApiCredentials(apiCredentials);

            string? listenKey = null;
            UpdateSubscription? updateSubscription = null;
            Task? keepAliveTask = null;

            try
            {
                var startUserStream = await binanceRestClient.SpotApi.Account.StartUserStreamAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (!startUserStream.Success)
                {
                    throw new Exception($"StartUserStream failed: {startUserStream.Error?.Code} {startUserStream.Error?.Message}");
                }

                listenKey = startUserStream?.Data;

                if (string.IsNullOrWhiteSpace(listenKey))
                {
                    throw new Exception("Listen key is null or empty.");
                }

                keepAliveTask = Task.Run(async () =>
                {
                    try
                    {
                        await RunListenKeyKeepAliveAsync(binanceRestClient, listenKey!, cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        // normal shutdown path
                    }
                    catch (Exception ex)
                    {
                        exception(ex);
                    }
                }, cancellationToken);

                var updateSubscriptionResult = await binanceSocketClient.SpotApi.Account.SubscribeToUserDataUpdatesAsync(
                    listenKey,
                    onAccountPositionMessage: positionsUpdate =>
                    {
                        try
                        {
                            var accountInfo = new AccountInfo
                            {
                                User = user,
                                Exchange = Exchange.Binance,
                                Time = positionsUpdate.Data.EventTime
                            };

                            foreach (var b in positionsUpdate.Data.Balances.Where(b => b.Available > 0 || b.Locked > 0))
                            {
                                accountInfo.Balances.Add(new Core.Model.AccountBalance
                                {
                                    Asset = b.Asset,
                                    Free = b.Available,
                                    Locked = b.Locked
                                });
                            }

                            callback(new AccountInfoEventArgs { AccountInfo = accountInfo });
                        }
                        catch (Exception ex)
                        {
                            exception(ex);
                        }
                    },
                    ct: cancellationToken
                ).ConfigureAwait(false);

                if (!updateSubscriptionResult.Success)
                {
                    throw new Exception($"Subscribe failed: {updateSubscriptionResult.Error?.Code} {updateSubscriptionResult.Error?.Message}");
                }

                updateSubscription = updateSubscriptionResult.Data;

                await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // normal shutdown path
            }
            catch (Exception ex)
            {
                exception(ex);
            }
            finally
            {
                try
                {
                    if (updateSubscription != null)
                    {
                        await updateSubscription.CloseAsync().ConfigureAwait(false);
                    }
                }
                catch { /* ignore */ }

                try
                {
                    if (!string.IsNullOrWhiteSpace(listenKey))
                    {
                        await binanceRestClient.SpotApi.Account.StopUserStreamAsync(listenKey!, CancellationToken.None).ConfigureAwait(false);
                    }
                }
                catch { /* ignore */ }

                binanceSocketClient.Dispose();
                binanceRestClient.Dispose();
            }
        }

        public Task SubscribeAggregateTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeCandlesticks(string symbol, CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(symbol)) throw new ArgumentNullException(nameof(symbol));
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));

            // This class maintains a correct local book (snapshot + diffs)
            var book = new BinanceSpotSymbolOrderBook(symbol, options =>
            {
                options.Limit = limit;
            });

            void Publish()
            {
                try
                {
                    var snapshot = book.Book; // local state (bids/asks levels)

                    var orderBook = new OrderBook
                    {
                        Symbol = symbol,
                        Exchange = Exchange.Binance,
                        Asks = snapshot.asks
                            .Select(x => new OrderBookPriceLevel { Price = x.Price, Quantity = x.Quantity })
                            .ToList(),
                        Bids = snapshot.bids
                            .Select(x => new OrderBookPriceLevel { Price = x.Price, Quantity = x.Quantity })
                            .ToList()
                    };

                    callback(new OrderBookEventArgs { OrderBook = orderBook });
                }
                catch (Exception ex)
                {
                    exception(ex);
                }
            }

            try
            {
                // Called when the local book changes
                book.OnOrderBookUpdate += _ => Publish();

                // Start syncing (downloads snapshot + subscribes to diffs)
                CallResult<bool> started = await book.StartAsync().ConfigureAwait(false);

                if (!started)
                { 
                    throw new Exception("Failed to start symbol order book (StartAsync returned false).");
                }

                // Publish an initial snapshot immediately
                Publish();

                await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // normal shutdown
            }
            catch (Exception ex)
            {
                exception(ex);
            }
            finally
            {
                try { await book.StopAsync().ConfigureAwait(false); } catch { /* ignore */ }
                book.Dispose();
            }
        }

        public Task SubscribeStatistics(Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task SubscribeStatistics(IEnumerable<string> symbols, Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            if (symbols == null) throw new ArgumentNullException(nameof(symbols));

            var list = symbols.Where(s => !string.IsNullOrWhiteSpace(s))
                              .Select(s => s.Trim().ToUpperInvariant())
                              .Distinct()
                              .ToList();

            if (list.Count == 0) throw new ArgumentException("No symbols provided", nameof(symbols));

            var socketClient = new BinanceSocketClient();
            var subscriptions = new List<UpdateSubscription>();

            try
            {
                foreach (var symbol in list)
                {
                    var subResult = await socketClient.SpotApi.ExchangeData.SubscribeToTickerUpdatesAsync(
                        symbol,
                        data =>
                        {
                            try
                            {
                                var t = data.Data;

                                var stats = new SymbolStats
                                {
                                    Symbol = t.Symbol,
                                    Exchange = Exchange.Binance,
                                    CloseTime = t.CloseTime,
                                    Volume = t.Volume,
                                    LowPrice = t.LowPrice,
                                    HighPrice = t.HighPrice,
                                    LastPrice = t.LastPrice,
                                    PriceChange = t.PriceChange,
                                    PriceChangePercent = t.PriceChangePercent
                                };

                                callback(new StatisticsEventArgs { Statistics = new List<SymbolStats> { stats } });
                            }
                            catch (Exception ex)
                            {
                                exception(ex);
                            }
                        },
                        ct: cancellationToken
                    ).ConfigureAwait(false);

                    if (!subResult.Success)
                    { 
                        throw new Exception($"Subscribe failed for {symbol}: {subResult.Error?.Code} {subResult.Error?.Message}");
                    }

                    subscriptions.Add(subResult.Data);
                }

                await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // normal shutdown
            }
            catch (Exception ex)
            {
                exception(ex);
            }
            finally
            {
                foreach (var s in subscriptions)
                {
                    try { await s.CloseAsync().ConfigureAwait(false); } catch { }
                }

                socketClient.Dispose();
            }
        }

        public async Task SubscribeTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(symbol)) throw new ArgumentNullException(nameof(symbol));

            var socketClient = new BinanceSocketClient();
            UpdateSubscription? subscription = null;

            try
            {
                var subResult = await socketClient.SpotApi.ExchangeData
                    .SubscribeToAggregatedTradeUpdatesAsync(
                        symbol,
                        data =>
                        {
                            try
                            {
                                var t = data.Data;

                                var trade = new Trade
                                {
                                    Id = t.Id,
                                    Exchange = Exchange.Binance,
                                    Symbol = t.Symbol,
                                    Price = t.Price,
                                    Time = t.TradeTime,
                                    Quantity = t.Quantity
                                };

                                callback(new TradeEventArgs
                                {
                                    Trades = new ITrade[] { trade }
                                });
                            }
                            catch (Exception ex)
                            {
                                exception(ex);
                            }
                        },
                        ct: cancellationToken
                    )
                    .ConfigureAwait(false);

                if (!subResult.Success)
                    throw new Exception($"Subscribe failed: {subResult.Error?.Code} {subResult.Error?.Message}");

                subscription = subResult.Data;

                await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // normal shutdown
            }
            catch (Exception ex)
            {
                exception(ex);
            }
            finally
            {
                try
                {
                    if (subscription != null)
                    { 
                        await subscription.CloseAsync().ConfigureAwait(false);
                    }
                }
                catch { /* ignore */ }

                socketClient.Dispose();
            }
        }

        private static BinanceRestClient GetBinanceRestClient(User user)
        {
            using var binanceRestClient = new BinanceRestClient();
            binanceRestClient.SetApiCredentials(new ApiCredentials(user.ApiKey, user.ApiSecret, user.ApiPassPhrase));
            return binanceRestClient;
        }

        private static async Task RunListenKeyKeepAliveAsync(
            BinanceRestClient restClient,
            string listenKey,
            CancellationToken cancellationToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(30));

            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                var keepAlive = await restClient.SpotApi.Account.KeepAliveUserStreamAsync(listenKey, cancellationToken)
                   .ConfigureAwait(false);

                if (!keepAlive.Success)
                {
                    throw new Exception($"KeepAliveUserStream failed: {keepAlive.Error?.Code} {keepAlive.Error?.Message}");
                }
            }
        }
    }
}