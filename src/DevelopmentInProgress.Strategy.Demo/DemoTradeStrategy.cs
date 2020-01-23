using DevelopmentInProgress.Strategy.Common;
using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Events;
using DevelopmentInProgress.TradeView.Interface.Extensions;
using DevelopmentInProgress.TradeView.Interface.Model;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DevelopmentInProgress.Strategy.Demo
{
    public class DemoTradeStrategy : TradeStrategyBase
    {
        private decimal buyIndicator;
        private decimal sellIndicator;
        private int tradeMovingAvarageSetLength;
        private TradeCache<DemoTradeCreator, DemoTrade> tradeCache;

        public override void UpdateParameters(string parameters)
        {
            var demoTradeStrategyParameters = JsonConvert.DeserializeObject<DemoTradeStrategyParameters>(parameters);

            buyIndicator = demoTradeStrategyParameters.BuyIndicator;
            sellIndicator = demoTradeStrategyParameters.SellIndicator;
            tradeMovingAvarageSetLength = demoTradeStrategyParameters.TradeMovingAvarageSetLength;
            suspend = demoTradeStrategyParameters.Suspend;

            StrategyNotification(new StrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = strategy.Name, Message = $"Parameter update : {parameters}", NotificationLevel = NotificationLevel.Information } });
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

                var trades = (from t in tradeEventArgs.Trades
                              orderby t.Time, t.Id
                              select t).ToList();

                var tradesCount = trades.Count();

                var demoTrades = new DemoTrade[tradesCount];
                var tradePrices = new decimal[tradesCount];

                var tradeSetLength = tradeMovingAvarageSetLength == 0 ? tradesCount : tradeMovingAvarageSetLength;

                for (int i = 0; i < tradesCount; i++)
                {
                    tradePrices[i] = trades[i].Price;

                    var priceMovingAverage = StrategyHelper.CalculateMovingAverage(i, tradePrices, tradeSetLength);

                    var buyIndicatorPrice = priceMovingAverage - (priceMovingAverage * buyIndicator);
                    var sellIndicatorPrice = priceMovingAverage + (priceMovingAverage * sellIndicator);
                    
                    var demoTrade = new DemoTrade
                    {
                        Symbol = trades[i].Symbol,                        
                        Exchange = trades[i].Exchange,
                        Id = trades[i].Id,
                        Price = trades[i].Price,
                        Quantity = trades[i].Quantity,
                        Time = trades[i].Time,
                        IsBuyerMaker = trades[i].IsBuyerMaker,
                        IsBestPriceMatch = trades[i].IsBestPriceMatch,
                        SmaPrice = priceMovingAverage,
                        BuyIndicatorPrice = buyIndicatorPrice,
                        SellIndicatorPrice = sellIndicatorPrice
                    };

                    demoTrades[i] = demoTrade;
                }

                var lastTrade = demoTrades[tradesCount - 1];

                if (!suspend)
                {
                    PlaceOrder(lastTrade);
                }

                message = JsonConvert.SerializeObject(demoTrades.ToList());
            }
            catch (Exception ex)
            {
                message = JsonConvert.SerializeObject(ex);
            }

            strategyNotification.Message = message;
            StrategyTradeNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        private void PlaceOrder(DemoTrade trade)
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

                    if (trade.Price > trade.SellIndicatorPrice)
                    {
                        orderSide = OrderSide.Sell;
                        stopPrice = trade.SellIndicatorPrice.HasRemainder() ? trade.SellIndicatorPrice.Trim(symbol.Price.Increment.GetPrecision()) : trade.SellIndicatorPrice;
                    }
                    else if (trade.Price < trade.BuyIndicatorPrice)
                    {
                        orderSide = OrderSide.Buy;
                        stopPrice = trade.BuyIndicatorPrice.HasRemainder() ? trade.BuyIndicatorPrice.Trim(symbol.Price.Increment.GetPrecision()) : trade.BuyIndicatorPrice;
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