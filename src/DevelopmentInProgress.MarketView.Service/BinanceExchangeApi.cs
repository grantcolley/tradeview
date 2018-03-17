using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using Binance;
using Binance.Cache;
using Binance.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Service
{
    public class BinanceExchangeApi : IExchangeApi
    {
        private IBinanceApi binanceApi;

        public BinanceExchangeApi()
        {
            binanceApi = new BinanceApi();
        }

        public async Task<Interface.Model.AccountInfo> GetAccountInfoAsync(Interface.Model.User user, CancellationToken cancellationToken)
        {
                var apiUser = new BinanceApiUser(user.ApiKey, user.ApiSecret);
                var result = await binanceApi.GetAccountInfoAsync(apiUser, 0, cancellationToken);
                var accountInfo = new Interface.Model.AccountInfo
                {
                    Commissions = new Interface.Model.AccountCommissions { Buyer = result.Commissions.Buyer, Maker = result.Commissions.Maker, Seller = result.Commissions.Seller, Taker = result.Commissions.Taker },
                    Status = new Interface.Model.AccountStatus { CanDeposit = result.Status.CanDeposit, CanTrade = result.Status.CanTrade, CanWithdraw = result.Status.CanWithdraw },
                    Time = result.Time,
                    Balances = new List<Interface.Model.AccountBalance>()
                };

                var balances = result.Balances.Where(b => b.Free > 0 || b.Locked > 0);
                foreach (var balance in balances)
                {
                    accountInfo.Balances.Add(new Interface.Model.AccountBalance { Asset = balance.Asset, Free = balance.Free, Locked = balance.Locked });
                }

                user.RateLimiter = new Interface.Model.RateLimiter { IsEnabled = result.User.RateLimiter.IsEnabled };
                accountInfo.User = user;
                return accountInfo;
        }

        public async Task<IEnumerable<Interface.Model.Symbol>> GetSymbolsAsync(CancellationToken cancellationToken)
        {
            var result = await binanceApi.GetSymbolsAsync(cancellationToken);
            var symbols = result.Select(s => new Interface.Model.Symbol
            {
                NotionalMinimumValue = s.NotionalMinimumValue,
                BaseAsset = new Interface.Model.Asset { Symbol = s.BaseAsset.Symbol, Precision = s.BaseAsset.Precision },
                Price = new Interface.Model.InclusiveRange { Increment = s.Price.Increment, Minimum = s.Price.Minimum, Maximum = s.Price.Maximum },
                Quantity = new Interface.Model.InclusiveRange { Increment = s.Quantity.Increment, Minimum = s.Quantity.Minimum, Maximum = s.Quantity.Maximum },
                QuoteAsset = new Interface.Model.Asset { Symbol = s.QuoteAsset.Symbol, Precision = s.QuoteAsset.Precision },
                Status = (Interface.Model.SymbolStatus)s.Status,
                IsIcebergAllowed = s.IsIcebergAllowed,
                OrderTypes = (IEnumerable<Interface.Model.OrderType>)s.OrderTypes
            }).ToList();
            return symbols;
        }

        public async Task<Interface.Model.OrderBook> GetOrderBookAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var result = await binanceApi.GetOrderBookAsync(symbol, limit, cancellationToken);
            var orderBook = NewOrderBook(result);
            return orderBook;
        }

        public void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            try
            {
                var orderBookCache = new OrderBookCache(binanceApi, new DepthWebSocketClient());
                orderBookCache.Subscribe(symbol, limit, e =>
                {
                    if(cancellationToken.IsCancellationRequested)
                    {
                        orderBookCache.Unsubscribe();
                        return;
                    }

                    try
                    {
                        var orderBook = NewOrderBook(e.OrderBook);
                        callback.Invoke(new OrderBookEventArgs { OrderBook = orderBook });
                    }
                    catch(Exception ex)
                    {
                        orderBookCache.Unsubscribe();
                        exception.Invoke(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                exception.Invoke(ex);
            }
        }

        public async Task<IEnumerable<Interface.Model.AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var trades = await binanceApi.GetAggregateTradesAsync(symbol, limit, cancellationToken);
            var aggregateTrades = trades.Select(at => NewAggregateTrade(at)).ToList();
            return aggregateTrades;
        }

        public void SubscribeAggregateTrades(string symbol, int limit, Action<AggregateTradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            try
            {
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
                        callback.Invoke(new AggregateTradeEventArgs { AggregateTrades = aggregateTrades });
                    }
                    catch (Exception ex)
                    {
                        aggregateTradeCache.Unsubscribe();
                        exception.Invoke(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                exception.Invoke(ex);
            }
        }

        public async Task<IEnumerable<Interface.Model.SymbolStats>> Get24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            var stats = await binanceApi.Get24HourStatisticsAsync(cancellationToken);
            var symbolsStats = stats.Select(s => NewSymbolStats(s)).ToList();
            return symbolsStats;
        }

        public void SubscribeStatistics(Action<StatiscticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            try
            {
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
                        callback.Invoke(new StatiscticsEventArgs { Statistics = symbolsStats });
                    }
                    catch (Exception ex)
                    {
                        symbolStatisticsCache.Unsubscribe();
                        exception.Invoke(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                exception.Invoke(ex);
            }
        }
        
        private Interface.Model.SymbolStats NewSymbolStats(SymbolStatistics s)
        {
            return new Interface.Model.SymbolStats
            {
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

        private Interface.Model.OrderBook NewOrderBook(OrderBook ob)
        {
            var orderBook = new Interface.Model.OrderBook
            {
                Symbol = ob.Symbol,
                LastUpdateId = ob.LastUpdateId,
                Top = new Interface.Model.OrderBookTop
                {
                    Symbol = ob.Top.Symbol,
                    Ask = new Interface.Model.OrderBookPriceLevel
                    {
                        Price = ob.Top.Ask.Price,
                        Quantity = ob.Top.Ask.Quantity
                    },
                    Bid = new Interface.Model.OrderBookPriceLevel
                    {
                        Price = ob.Top.Bid.Price,
                        Quantity = ob.Top.Bid.Quantity
                    }
                }
            };

            orderBook.Asks = from ask in ob.Asks select new Interface.Model.OrderBookPriceLevel { Price = ask.Price, Quantity = ask.Quantity };
            orderBook.Bids = from bid in ob.Bids select new Interface.Model.OrderBookPriceLevel { Price = bid.Price, Quantity = bid.Quantity };

            return orderBook;
        }

        private Interface.Model.AggregateTrade NewAggregateTrade(AggregateTrade at)
        {
            return new Interface.Model.AggregateTrade
            {
                Symbol = at.Symbol,
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
    }
}