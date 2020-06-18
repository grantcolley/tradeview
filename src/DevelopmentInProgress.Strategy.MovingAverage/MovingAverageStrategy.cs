using DevelopmentInProgress.Strategy.Common;
using DevelopmentInProgress.Strategy.Common.Parameter;
using DevelopmentInProgress.Strategy.Common.StrategyTrade;
using DevelopmentInProgress.Strategy.Common.TradeCreator;
using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Extensions;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Model;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
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

            var strategyParameters = JsonConvert.DeserializeObject<MovingAverageTradeParameters>(parameters);

            if (tradeCache == null)
            {
                tradeCache = new TradeCache<MovingAverageTradeCreator, MovingAverageTrade, MovingAverageTradeParameters>(strategyParameters.TradeRange);
            }

            if (movingAverageTradeParameters == null
               || !strategyParameters.MovingAvarageRange.Equals(movingAverageTradeParameters.MovingAvarageRange)
               || !strategyParameters.SellIndicator.Equals(movingAverageTradeParameters.SellIndicator)
               || !strategyParameters.BuyIndicator.Equals(movingAverageTradeParameters.BuyIndicator))
            {
                movingAverageTradeParameters = strategyParameters;
                Strategy.Parameters = parameters;
                tradeCache.TradeCreator.Reset(movingAverageTradeParameters);
            }

            Suspend = movingAverageTradeParameters.Suspend;

            StrategyParameterUpdateNotification(new StrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = Strategy.Name, Message = parameters, NotificationLevel = NotificationLevel.ParameterUpdate } });

            tcs.SetResult(true);

            return await tcs.Task.ConfigureAwait(false);
        }

        public override void SubscribeTrades(TradeEventArgs tradeEventArgs)
        {
            if (tradeEventArgs == null)
            {
                throw new ArgumentNullException(nameof(tradeEventArgs));
            }

            if (Strategy == null)
            {
                return;
            }

            var previousLastTrade = tradeCache.GetLastTrade();

            ITrade[] trades;
            MovingAverageTrade[] movingAverageTrades;

            if (previousLastTrade == null)
            {
                trades = (from t in tradeEventArgs.Trades
                          orderby t.Time, t.Id
                          select t).ToArray();
                movingAverageTrades = tradeCache.AddRange(trades);
            }
            else
            {
                trades = (from t in tradeEventArgs.Trades
                          where t.Time > previousLastTrade.Time && t.Id > previousLastTrade.Id
                          orderby t.Time, t.Id
                          select t).ToArray();
                movingAverageTrades = tradeCache.AddRange(trades);
            }

            var lastTrade = tradeCache.GetLastTrade();

            if (!Suspend)
            {
                PlaceOrder(lastTrade);
            }

            var strategyNotification = new StrategyNotification { Name = Strategy.Name, NotificationLevel = NotificationLevel.Trade };
            strategyNotification.Message = JsonConvert.SerializeObject(movingAverageTrades);
            StrategyTradeNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Catch exceptions raised in validating an order or placing an order and feed it back to the subscriber.")]
        private void PlaceOrder(MovingAverageTrade trade)
        {
            if (AccountInfo == null
                || PlacingOrder)
            {
                return;
            }

            lock (AccountLock)
            {
                var symbol = ExchangeSymbols[Exchange.Binance].Single(s => s.ExchangeSymbol.Equals(trade.Symbol, StringComparison.Ordinal));

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

                var quoteAssetBalance = AccountInfo.Balances.FirstOrDefault(b => b.Asset.Equals(symbol.QuoteAsset.Symbol, StringComparison.Ordinal));
                var baseAssetBalance = AccountInfo.Balances.FirstOrDefault(b => b.Asset.Equals(symbol.BaseAsset.Symbol, StringComparison.Ordinal));

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

                    try
                    {
                        symbol.ValidateClientOrder(clientOrder);

                        var strategySymbol = Strategy.StrategySubscriptions.SingleOrDefault(ss => ss.Symbol.Equals(trade.Symbol, StringComparison.Ordinal));
                        var user = new User { ApiKey = strategySymbol.ApiKey, ApiSecret = strategySymbol.SecretKey, Exchange = Exchange.Binance };
                        var order = ExchangeServices[Exchange.Binance].PlaceOrder(user.Exchange, user, clientOrder).Result;

                        var message = $"{clientOrder.Symbol} {order.Price} {clientOrder.Quantity} {clientOrder.Side} {clientOrder.Type}";

                        StrategyNotification(new StrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = Strategy.Name, Message = message, NotificationLevel = NotificationLevel.Information } });
                    }
                    catch (Exception ex)
                    {
                        PlacingOrder = false;
                        StrategyNotification(new StrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = Strategy.Name, Message = ex.Message, NotificationLevel = NotificationLevel.Information } });
                    }
                }
                else
                {
                    PlacingOrder = false;
                }
            }
        }
    }
}