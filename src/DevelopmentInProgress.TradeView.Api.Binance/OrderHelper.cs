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
            return orderType switch
            {
                OrderType.Market => new MarketOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity },
                OrderType.StopLoss => new StopLossOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, StopPrice = clientOrder.StopPrice },
                OrderType.TakeProfit => new TakeProfitOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, StopPrice = clientOrder.StopPrice },
                OrderType.Limit => new LimitOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, Price = clientOrder.Price },
                OrderType.LimitMaker => new LimitMakerOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, Price = clientOrder.Price },
                OrderType.StopLossLimit => new StopLossLimitOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, StopPrice = clientOrder.StopPrice, Price = clientOrder.Price },
                OrderType.TakeProfitLimit => new TakeProfitLimitOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, StopPrice = clientOrder.StopPrice, Price = clientOrder.Price },
                _ => throw new ArgumentOutOfRangeException($"Order type not supported for {nameof(clientOrder)}"),
            };
        }
    }
}
