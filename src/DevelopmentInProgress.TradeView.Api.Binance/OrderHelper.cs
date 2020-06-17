using Binance;
using System;

namespace DevelopmentInProgress.TradeView.Api.Binance
{
    public static class OrderHelper
    {
        public static ClientOrder GetOrder(BinanceApiUser apiUser, Core.Model.ClientOrder clientOrder)
        {
            if (apiUser == null)
            {
                throw new ArgumentNullException(nameof(apiUser));
            }

            if (clientOrder == null)
            {
                throw new ArgumentNullException(nameof(clientOrder));
            }

            var orderType = (OrderType)clientOrder.Type;
            switch (orderType)
            {
                case OrderType.Market:
                    return new MarketOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity };
                case OrderType.StopLoss:
                    return new StopLossOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, StopPrice = clientOrder.StopPrice };
                case OrderType.TakeProfit:
                    return new TakeProfitOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, StopPrice = clientOrder.StopPrice };
                case OrderType.Limit:
                    return new LimitOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, Price = clientOrder.Price };
                case OrderType.LimitMaker:
                    return new LimitMakerOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, Price = clientOrder.Price };
                case OrderType.StopLossLimit:
                    return new StopLossLimitOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, StopPrice = clientOrder.StopPrice, Price = clientOrder.Price };
                case OrderType.TakeProfitLimit:
                    return new TakeProfitLimitOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, StopPrice = clientOrder.StopPrice, Price = clientOrder.Price };
                default:
                    throw new ArgumentOutOfRangeException($"Order type not supported for {nameof(clientOrder)}");
            }
        }
    }
}
