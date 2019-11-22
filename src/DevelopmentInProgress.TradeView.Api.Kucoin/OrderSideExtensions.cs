using DevelopmentInProgress.TradeView.Interface.Model;
using Kucoin.Net.Objects;
using System;

namespace DevelopmentInProgress.TradeView.Api.Kucoin
{
    public static class OrderSideExtensions
    {
        public static KucoinOrderSide ToKucoinOrderSide(this OrderSide order)
        {
            switch (order)
            {
                case OrderSide.Buy:
                    return KucoinOrderSide.Buy;
                case OrderSide.Sell:
                    return KucoinOrderSide.Sell;
                default:
                    throw new NotImplementedException();
            }
        }

        public static OrderSide ToMarketViewOrderSide(this KucoinOrderSide order)
        {
            switch (order)
            {
                case KucoinOrderSide.Buy:
                    return OrderSide.Buy;
                case KucoinOrderSide.Sell:
                    return OrderSide.Sell;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
