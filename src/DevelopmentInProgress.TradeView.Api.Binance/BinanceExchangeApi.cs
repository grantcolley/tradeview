using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using Binance;
using Binance.Cache;
using Binance.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Core.Enums;
using System.Globalization;

namespace DevelopmentInProgress.TradeView.Api.Binance
{
    public class BinanceExchangeApi : IExchangeApi
    {
        public async Task<Core.Model.Order> PlaceOrder(Core.Model.User user, Core.Model.ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var binanceApi = new BinanceApi();
            using (var apiUser = new BinanceApiUser(user.ApiKey, user.ApiSecret))
            {
                var order = OrderHelper.GetOrder(apiUser, clientOrder);
                var result = await binanceApi.PlaceAsync(order).ConfigureAwait(false);
                return NewOrder(user, result);
            }
        }

        public async Task<string> CancelOrderAsync(Core.Model.User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (orderId == null)
            {
                throw new ArgumentNullException(nameof(orderId));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var binanceApi = new BinanceApi();
            var id = Convert.ToInt64(orderId);
            using (var apiUser = new BinanceApiUser(user.ApiKey, user.ApiSecret))
            {
                var result = await binanceApi.CancelOrderAsync(apiUser, symbol, id, newClientOrderId, recWindow, cancellationToken).ConfigureAwait(false);
                return result;
            }
        }

        public async Task<Core.Model.AccountInfo> GetAccountInfoAsync(Core.Model.User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var binanceApi = new BinanceApi();
            using (var apiUser = new BinanceApiUser(user.ApiKey, user.ApiSecret))
            {
                var result = await binanceApi.GetAccountInfoAsync(apiUser, 0, cancellationToken).ConfigureAwait(false);
                var accountInfo = GetAccountInfo(result);
                user.RateLimiter = new Core.Model.RateLimiter { IsEnabled = result.User.RateLimiter.IsEnabled };
                accountInfo.User = user;
                return accountInfo;
            }
        }

        public async Task<IEnumerable<Core.Model.AccountTrade>> GetAccountTradesAsync(Core.Model.User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var binanceApi = new BinanceApi();
            using (var apiUser = new BinanceApiUser(user.ApiKey, user.ApiSecret))
            {
                var result = await binanceApi.GetAccountTradesAsync(apiUser, symbol, startDate, endDate, recWindow, cancellationToken).ConfigureAwait(false);
                var accountTrades = result.Select(at => NewAccountTrade(at)).ToList();
                return accountTrades;
            }
        }

        public async Task<IEnumerable<Core.Model.Candlestick>> GetCandlesticksAsync(string symbol, Core.Model.CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = 0, CancellationToken token = default(CancellationToken))
        {
            var binanceApi = new BinanceApi();
            var candlestickInterval = interval.ToBinanceCandlestickInterval();
            var results = await binanceApi.GetCandlesticksAsync(symbol, candlestickInterval, startTime, endTime, limit, token).ConfigureAwait(false);
            var candlesticks = results.Select(cs => NewCandlestick(cs)).ToList();
            return candlesticks;
        }

        public async Task<IEnumerable<Core.Model.Symbol>> GetSymbols24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            var symbols = await GetSymbolsAsync(cancellationToken).ConfigureAwait(false);
            var symbolStatistics = await Get24HourStatisticsAsync(cancellationToken).ConfigureAwait(false);

            Core.Model.Symbol f(Core.Model.Symbol s, Core.Model.SymbolStats ss)
            {
                s.SymbolStatistics = ss;
                return s;
            };

            var updatedSymbols = (from s in symbols
                                  join ss in symbolStatistics on $"{s.BaseAsset.Symbol}{s.QuoteAsset.Symbol}" equals ss.Symbol
                                  select f(s, ss)).ToList();

            return updatedSymbols;
        }

        public async Task<IEnumerable<Core.Model.Symbol>> GetSymbolsAsync(CancellationToken cancellationToken)
        {
            var binanceApi = new BinanceApi();
            var result = await binanceApi.GetSymbolsAsync(cancellationToken).ConfigureAwait(false);
            var symbols = result.Select(s => new Core.Model.Symbol
            {
                Name = $"{s.BaseAsset.Symbol}{s.QuoteAsset.Symbol}",
                ExchangeSymbol = $"{s.BaseAsset.Symbol}{s.QuoteAsset.Symbol}",
                Exchange = Exchange.Binance,
                NotionalMinimumValue = s.NotionalMinimumValue,
                BaseAsset = new Core.Model.Asset { Symbol = s.BaseAsset.Symbol, Precision = s.BaseAsset.Precision },
                Price = new Core.Model.InclusiveRange { Increment = s.Price.Increment /*, Minimum = s.Price.Minimum, Maximum = s.Price.Maximum*/ }, // HACK : remove Price Min and Max because it realtime calcs hits performance.
                Quantity = new Core.Model.InclusiveRange { Increment = s.Quantity.Increment, Minimum = s.Quantity.Minimum, Maximum = s.Quantity.Maximum },
                QuoteAsset = new Core.Model.Asset { Symbol = s.QuoteAsset.Symbol, Precision = s.QuoteAsset.Precision },
                Status = (Core.Model.SymbolStatus)s.Status,
                IsIcebergAllowed = s.IsIcebergAllowed,
                OrderTypes = (IEnumerable<Core.Model.OrderType>)s.OrderTypes
            }).ToList();
            return symbols;
        }

        public async Task<IEnumerable<Core.Model.SymbolStats>> Get24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            var binanceApi = new BinanceApi();
            var stats = await binanceApi.Get24HourStatisticsAsync(cancellationToken).ConfigureAwait(false);
            var symbolsStats = stats.Select(s => NewSymbolStats(s)).ToList();
            return symbolsStats;
        }

        public async Task<Core.Model.OrderBook> GetOrderBookAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var binanceApi = new BinanceApi();
            var result = await binanceApi.GetOrderBookAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            var orderBook = NewOrderBook(result);
            return orderBook;
        }

        public async Task<IEnumerable<Core.Model.AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var binanceApi = new BinanceApi();
            var trades = await binanceApi.GetAggregateTradesAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            var aggregateTrades = trades.Select(at => NewAggregateTrade(at)).ToList();
            return aggregateTrades;
        }

        public async Task<IEnumerable<Core.Model.Trade>> GetTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var binanceApi = new BinanceApi();
            var result = await binanceApi.GetTradesAsync(symbol, limit, cancellationToken).ConfigureAwait(false);
            var trades = result.Select(t => NewTrade(t)).ToList();
            return trades;
        }

        public async Task<IEnumerable<Core.Model.Order>> GetOpenOrdersAsync(Core.Model.User user, string symbol = null, long recWindow = 0, Action<Exception> exception = default(Action<Exception>), CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var binanceApi = new BinanceApi();
            using (var apiUser = new BinanceApiUser(user.ApiKey, user.ApiSecret))
            {
                var result = await binanceApi.GetOpenOrdersAsync(apiUser, symbol, recWindow, cancellationToken).ConfigureAwait(false);
                var orders = result.Select(o => NewOrder(user, o)).ToList();
                return orders;
            }
        }

        public Task SubscribeCandlesticks(string symbol, Core.Model.CandlestickInterval candlestickInterval, int limit, Action<CandlestickEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                var binanceApi = new BinanceApi();
                var interval = candlestickInterval.ToBinanceCandlestickInterval();
                var canclestickCache = new CandlestickCache(binanceApi, new CandlestickWebSocketClient());
                canclestickCache.Subscribe(symbol, interval, limit, e =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        canclestickCache.Unsubscribe();
                        return;
                    }

                    try
                    {
                        var candlesticks = (from c in e.Candlesticks select NewCandlestick(c)).ToList();
                        callback.Invoke(new CandlestickEventArgs { Candlesticks = candlesticks });
                    }
                    catch (Exception ex)
                    {
                        canclestickCache.Unsubscribe();
                        exception.Invoke(ex);
                        return;
                    }
                });

                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        public Task SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                var binanceApi = new BinanceApi();
                var orderBookCache = new OrderBookCache(binanceApi, new DepthWebSocketClient());
                orderBookCache.Subscribe(symbol, limit, e =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        orderBookCache.Unsubscribe();
                        return;
                    }

                    try
                    {
                        var orderBook = NewOrderBook(e.OrderBook);
                        callback.Invoke(new OrderBookEventArgs { OrderBook = orderBook });
                    }
                    catch (Exception ex)
                    {
                        orderBookCache.Unsubscribe();
                        exception.Invoke(ex);
                        return;
                    }
                });

                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        public Task SubscribeAggregateTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                var binanceApi = new BinanceApi();
                var aggregateTradeCache = new AggregateTradeCache(binanceApi, new AggregateTradeWebSocketClient());
                aggregateTradeCache.Subscribe(symbol, limit, e =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        aggregateTradeCache.Unsubscribe();
                        return;
                    }

                    try
                    {
                        var aggregateTrades = e.Trades.Select(at => NewAggregateTrade(at)).ToList();
                        callback.Invoke(new TradeEventArgs { Trades = aggregateTrades });
                    }
                    catch (Exception ex)
                    {
                        aggregateTradeCache.Unsubscribe();
                        exception.Invoke(ex);
                        return;
                    }
                });

                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        public Task SubscribeTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                var binanceApi = new BinanceApi();
                var tradeCache = new TradeCache(binanceApi, new TradeWebSocketClient());
                tradeCache.Subscribe(symbol, limit, e =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        tradeCache.Unsubscribe();
                        return;
                    }

                    try
                    {
                        var trades = e.Trades.Select(t => NewTrade(t)).ToList();
                        callback.Invoke(new TradeEventArgs { Trades = trades });
                    }
                    catch (Exception ex)
                    {
                        tradeCache.Unsubscribe();
                        exception.Invoke(ex);
                        return;
                    }
                });

                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        public Task SubscribeStatistics(IEnumerable<string> symbols, Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            return SubscribeStatistics(callback, exception, cancellationToken);
        }

        public Task SubscribeStatistics(Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                var binanceApi = new BinanceApi();
                var symbolStatisticsCache = new SymbolStatisticsCache(binanceApi, new SymbolStatisticsWebSocketClient());
                symbolStatisticsCache.Subscribe(e =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        symbolStatisticsCache.Unsubscribe();
                        return;
                    }

                    try
                    {
                        var symbolsStats = e.Statistics.Select(s => NewSymbolStats(s)).ToList();
                        callback.Invoke(new StatisticsEventArgs { Statistics = symbolsStats });
                    }
                    catch (Exception ex)
                    {
                        symbolStatisticsCache.Unsubscribe();
                        exception.Invoke(ex);
                        return;
                    }
                });
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        public async Task SubscribeAccountInfo(Core.Model.User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var binanceApi = new BinanceApi();
            var apiUser = new BinanceApiUser(user.ApiKey, user.ApiSecret);
            var streamControl = new UserDataWebSocketStreamControl(binanceApi);
            var listenKey = await streamControl.OpenStreamAsync(apiUser).ConfigureAwait(false);

            var accountInfoCache = new AccountInfoCache(binanceApi, new UserDataWebSocketClient());

            accountInfoCache.Subscribe(listenKey, apiUser, e =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    apiUser.Dispose();
                    accountInfoCache.Unsubscribe();
                    return;
                }

                try
                {
                    var accountInfo = GetAccountInfo(e.AccountInfo);
                    accountInfo.User = user;
                    callback.Invoke(new AccountInfoEventArgs { AccountInfo = accountInfo });
                }
                catch (Exception ex)
                {
                    apiUser.Dispose();
                    accountInfoCache.Unsubscribe();
                    exception.Invoke(ex);
                    return;
                }
            });
        }

        private static Core.Model.AccountInfo GetAccountInfo(AccountInfo a)
        {
            var accountInfo = new Core.Model.AccountInfo
            {
                Exchange = Exchange.Binance,
                Commissions = new Core.Model.AccountCommissions { Buyer = a.Commissions.Buyer, Maker = a.Commissions.Maker, Seller = a.Commissions.Seller, Taker = a.Commissions.Taker },
                Status = new Core.Model.AccountStatus { CanDeposit = a.Status.CanDeposit, CanTrade = a.Status.CanTrade, CanWithdraw = a.Status.CanWithdraw },
                Time = a.Time
            };

            var balances = a.Balances.Where(b => b.Free > 0 || b.Locked > 0);
            foreach (var balance in balances)
            {
                accountInfo.Balances.Add(new Core.Model.AccountBalance { Asset = balance.Asset, Free = balance.Free, Locked = balance.Locked });
            }

            return accountInfo;
        }

        private static Core.Model.Order NewOrder(Core.Model.User user, Order o)
        {
            return new Core.Model.Order
            {
                User = user,
                Symbol = o.Symbol,
                Exchange = Exchange.Binance,
                Id = o.Id.ToString(CultureInfo.InvariantCulture),
                ClientOrderId = o.ClientOrderId,
                Price = o.Price,
                OriginalQuantity = o.OriginalQuantity,
                ExecutedQuantity = o.ExecutedQuantity,
                Status = (Core.Model.OrderStatus)o.Status,
                TimeInForce = (Core.Model.TimeInForce)o.TimeInForce,
                Type = (Core.Model.OrderType)o.Type,
                Side = (Core.Model.OrderSide)o.Side,
                StopPrice = o.StopPrice,
                IcebergQuantity = o.IcebergQuantity,
                Time = o.Time,
                IsWorking = o.IsWorking,
                Fills = o.Fills?.Select(f => new Core.Model.Fill
                {
                    Price = f.Price,
                    Quantity = f.Quantity,
                    Commission = f.Commission,
                    CommissionAsset = f.CommissionAsset,
                    TradeId = f.TradeId
                })
            };
        }

        private static Core.Model.SymbolStats NewSymbolStats(SymbolStatistics s)
        {
            return new Core.Model.SymbolStats
            {
                Exchange = Exchange.Binance,
                FirstTradeId = s.FirstTradeId,
                CloseTime = s.CloseTime,
                OpenTime = s.OpenTime,
                QuoteVolume = s.QuoteVolume,
                Volume = s.Volume,
                LowPrice = s.LowPrice,
                HighPrice = s.HighPrice,
                OpenPrice = s.OpenPrice,
                AskQuantity = s.AskQuantity,
                AskPrice = s.AskPrice,
                BidQuantity = s.BidQuantity,
                BidPrice = s.BidPrice,
                LastQuantity = s.LastQuantity,
                LastPrice = s.LastPrice,
                PreviousClosePrice = s.PreviousClosePrice,
                WeightedAveragePrice = s.WeightedAveragePrice,
                PriceChangePercent = s.PriceChangePercent,
                PriceChange = s.PriceChange,
                Period = s.Period,
                Symbol = s.Symbol,
                LastTradeId = s.LastTradeId,
                TradeCount = s.TradeCount
            };
        }

        private static Core.Model.OrderBook NewOrderBook(OrderBook ob)
        {
            var orderBook = new Core.Model.OrderBook
            {
                Symbol = ob.Symbol,
                Exchange = Exchange.Binance,
                LastUpdateId = ob.LastUpdateId
            };

            orderBook.Asks = (from ask in ob.Asks select new Core.Model.OrderBookPriceLevel { Price = ask.Price, Quantity = ask.Quantity }).ToList();
            orderBook.Bids = (from bid in ob.Bids select new Core.Model.OrderBookPriceLevel { Price = bid.Price, Quantity = bid.Quantity }).ToList();

            return orderBook;
        }

        private static Core.Model.AggregateTrade NewAggregateTrade(AggregateTrade at)
        {
            return new Core.Model.AggregateTrade
            {
                Symbol = at.Symbol,
                Exchange = Exchange.Binance,
                Id = at.Id,
                Price = at.Price,
                Quantity = at.Quantity,
                Time = at.Time,
                IsBuyerMaker = at.IsBuyerMaker,
                IsBestPriceMatch = at.IsBestPriceMatch,
                FirstTradeId = at.FirstTradeId,
                LastTradeId = at.LastTradeId
            };
        }

        private static Core.Model.Trade NewTrade(Trade t)
        {
            return new Core.Model.Trade
            {
                Symbol = t.Symbol,
                Exchange = Exchange.Binance,
                Id = t.Id,
                Price = t.Price,
                Quantity = t.Quantity,
                Time = t.Time,
                BuyerOrderId = t.BuyerOrderId,
                SellerOrderId = t.SellerOrderId,
                IsBuyerMaker = t.IsBuyerMaker,
                IsBestPriceMatch = t.IsBestPriceMatch
            };
        }

        private static Core.Model.Candlestick NewCandlestick(Candlestick c)
        {
            var interval = c.Interval.ToTradeViewCandlestickInterval();

            return new Core.Model.Candlestick
            {
                Symbol = c.Symbol,
                Exchange = Exchange.Binance,
                Interval = interval,
                OpenTime = c.OpenTime,
                Open = c.Open,
                High = c.High,
                Low = c.Low,
                Close = c.Close,
                Volume = c.Volume,
                CloseTime = c.CloseTime,
                QuoteAssetVolume = c.QuoteAssetVolume,
                NumberOfTrades = c.NumberOfTrades,
                TakerBuyBaseAssetVolume = c.TakerBuyBaseAssetVolume,
                TakerBuyQuoteAssetVolume = c.TakerBuyQuoteAssetVolume
            };
        }

        private static Core.Model.AccountTrade NewAccountTrade(AccountTrade t)
        {
            return new Core.Model.AccountTrade
            {
                Symbol = t.Symbol,
                Exchange = Exchange.Binance,
                Id = t.Id,
                Price = t.Price,
                Quantity = t.Quantity,
                Time = t.Time,
                BuyerOrderId = t.BuyerOrderId,
                SellerOrderId = t.SellerOrderId,
                IsBuyerMaker = t.IsBuyerMaker,
                IsBestPriceMatch = t.IsBestPriceMatch,
                OrderId = t.OrderId,
                QuoteQuantity = t.QuoteQuantity,
                Commission = t.Commission,
                CommissionAsset = t.CommissionAsset,
                IsBuyer = t.IsBuyer,
                IsMaker = t.IsMaker
            };
        }
    }
}