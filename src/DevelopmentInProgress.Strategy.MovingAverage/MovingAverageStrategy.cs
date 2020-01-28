using DevelopmentInProgress.Strategy.Common;
using DevelopmentInProgress.Strategy.Common.Parameter;
using DevelopmentInProgress.Strategy.Common.StrategyTrade;
using DevelopmentInProgress.Strategy.Common.TradeCreator;
using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Events;
using DevelopmentInProgress.TradeView.Interface.Extensions;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Interface.Model;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Strategy.MovingAverage
{
    public class MovingAverageStrategy : TradeStrategyBase
    {
        private MovingAverageTradeParameters movingAverageTradeParameters;
        private TradeCache<MovingAverageTradeCreator, MovingAverageTrade, MovingAverageTradeParameters> tradeCache;

        public override async Task<bool> TryUpdateStrategyAsync(string parameters)
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                var strategyParameters = JsonConvert.DeserializeObject<MovingAverageTradeParameters>(parameters);

                if(tradeCache == null)
                {
                    tradeCache = new TradeCache<MovingAverageTradeCreator, MovingAverageTrade, MovingAverageTradeParameters>(strategyParameters.TradeRange);
                }

                if(movingAverageTradeParameters == null 
                   || !strategyParameters.MovingAvarageRange.Equals(movingAverageTradeParameters.MovingAvarageRange)
                   || !strategyParameters.SellIndicator.Equals(movingAverageTradeParameters.SellIndicator)
                   || !strategyParameters.BuyIndicator.Equals(movingAverageTradeParameters.BuyIndicator))
                {
                    movingAverageTradeParameters = strategyParameters;
                    tradeCache.TradeCreator.Reset(movingAverageTradeParameters);
                }

                suspend = movingAverageTradeParameters.Suspend;

                StrategyNotification(new StrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = strategy.Name, Message = $"Parameter update : {parameters}", NotificationLevel = NotificationLevel.Information } });

                tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return await tcs.Task;
        }

        public override void SubscribeTrades(TradeEventArgs tradeEventArgs)
        {
            if (strategy == null)
            {
                return;
            }

            var strategyNotification = new StrategyNotification { Name = strategy.Name, NotificationLevel = NotificationLevel.Trade };
            string message;

            try
            {
                var previousLastTrade = tradeCache.GetLastTrade();

                ITrade[] trades;

                if (previousLastTrade == null)
                {
                    trades = (from t in tradeEventArgs.Trades
                                  orderby t.Time, t.Id
                                  select t).ToArray();
                    tradeCache.AddRange(trades);
                }
                else
                {
                    trades = (from t in tradeEventArgs.Trades
                                  where t.Time > previousLastTrade.Time && t.Id > previousLastTrade.Id
                                  orderby t.Time, t.Id
                                  select t).ToArray();
                    tradeCache.AddRange(trades);
                }

                var lastTrade = tradeCache.GetLastTrade();

                if (!suspend)
                {
                    PlaceOrder(lastTrade);
                }

                message = JsonConvert.SerializeObject(trades);
            }
            catch (Exception ex)
            {
                message = JsonConvert.SerializeObject(ex);
            }

            strategyNotification.Message = message;
            StrategyTradeNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        private void PlaceOrder(MovingAverageTrade trade)
        {
            if (accountInfo == null
                || placingOrder)
            {
                return;
            }

            lock (accountLock)
            {
                try
                {                    
                    var symbol = exchangeSymbols[Exchange.Binance].Single(s => s.ExchangeSymbol.Equals(trade.Symbol));

                    OrderSide orderSide;
                    decimal stopPrice = 0m;

                    if (trade.Price > trade.SellPrice)
                    {
                        orderSide = OrderSide.Sell;
                        stopPrice = trade.SellPrice.HasRemainder() ? trade.SellPrice.Trim(symbol.Price.Increment.GetPrecision()) : trade.SellPrice;
                    }
                    else if (trade.Price < trade.BuyPrice)
                    {
                        orderSide = OrderSide.Buy;
                        stopPrice = trade.BuyPrice.HasRemainder() ? trade.BuyPrice.Trim(symbol.Price.Increment.GetPrecision()) : trade.BuyPrice;
                    }
                    else
                    {
                        return;
                    }

                    decimal quantity = 0m;

                    var quoteAssetBalance = accountInfo.Balances.FirstOrDefault(b => b.Asset.Equals(symbol.QuoteAsset.Symbol));
                    var baseAssetBalance = accountInfo.Balances.FirstOrDefault(b => b.Asset.Equals(symbol.BaseAsset.Symbol));

                    if (orderSide.Equals(OrderSide.Buy)
                        && quoteAssetBalance.Free > 0)
                    {
                        quantity = quoteAssetBalance.Free / trade.Price;
                    }
                    else if (orderSide.Equals(OrderSide.Sell)
                        && baseAssetBalance.Free > 0)
                    {
                        quantity = baseAssetBalance.Free;
                    }

                    var trimmedQuantity = quantity.HasRemainder() ? quantity.Trim(symbol.Quantity.Increment.GetPrecision()) : quantity;
                    var trimmedPrice = trade.Price.HasRemainder() ? trade.Price.Trim(symbol.Price.Increment.GetPrecision()) : trade.Price;

                    var value = trimmedQuantity * trimmedPrice;

                    if (value > symbol.NotionalMinimumValue)
                    {
                        var clientOrder = new ClientOrder
                        {
                            Symbol = trade.Symbol,
                            Price = trimmedPrice,
                            StopPrice = stopPrice,
                            Type = OrderType.Limit,
                            Side = orderSide,
                            Quantity = trimmedQuantity,
                            QuoteAccountBalance = quoteAssetBalance,
                            BaseAccountBalance = baseAssetBalance
                        };

                        symbol.ValidateClientOrder(clientOrder);

                        var strategySymbol = strategy.StrategySubscriptions.SingleOrDefault(ss => ss.Symbol.Equals(trade.Symbol));
                        var user = new User { ApiKey = strategySymbol.ApiKey, ApiSecret = strategySymbol.SecretKey, Exchange = Exchange.Binance };
                        var order = exchangeServices[Exchange.Binance].PlaceOrder(user.Exchange, user, clientOrder).Result;

                        var message = $"{clientOrder.Symbol} {order.Price} {clientOrder.Quantity} {clientOrder.Side} {clientOrder.Type}";

                        StrategyNotification(new StrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = strategy.Name, Message = message, NotificationLevel = NotificationLevel.Information } });
                    }
                    else
                    {
                        placingOrder = false;
                    }
                }
                catch (Exception ex)
                {
                    placingOrder = false;
                    StrategyNotification(new StrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = strategy.Name, Message = ex.Message, NotificationLevel = NotificationLevel.Information } });
                }
            }
        }
    }
}