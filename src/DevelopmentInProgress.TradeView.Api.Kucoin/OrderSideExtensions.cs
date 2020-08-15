using DevelopmentInProgress.TradeView.Core.Model;
using Kucoin.Net.Objects;
using System;

namespace DevelopmentInProgress.TradeView.Api.Kucoin
{
    public static class OrderSideExtensions
    {
        public static KucoinOrderSide ToKucoinOrderSide(this OrderSide order)
        {
            return order switch
            {
                OrderSide.Buy => KucoinOrderSide.Buy,
                OrderSide.Sell => KucoinOrderSide.Sell,
                _ => throw new NotImplementedException(),
            };
        }

        public static OrderSide ToTradeViewOrderSide(this KucoinOrderSide order)
        {
            return order switch
            {
                KucoinOrderSide.Buy => OrderSide.Buy,
                KucoinOrderSide.Sell => OrderSide.Sell,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
