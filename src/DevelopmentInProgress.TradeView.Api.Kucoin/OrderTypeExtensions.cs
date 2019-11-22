using DevelopmentInProgress.TradeView.Interface.Model;
using Kucoin.Net.Objects;
using System;

namespace DevelopmentInProgress.TradeView.Api.Kucoin
{
    public static class OrderTypeExtensions
    {
        public static KucoinNewOrderType ToKucoinNewOrderType(this OrderType order)
        {
            switch (order)
            {
                case OrderType.Limit:
                    return KucoinNewOrderType.Limit;
                case OrderType.Market:
                    return KucoinNewOrderType.Market;
                default:
                    throw new NotImplementedException();
            }
        }

        public static OrderType ToTradeViewOrderType(this KucoinOrderType order)
        {
            switch (order)
            {
                case KucoinOrderType.Limit:
                    return OrderType.Limit;
                case KucoinOrderType.LimitStop:
                    return OrderType.StopLossLimit;
                case KucoinOrderType.Market:
                    return OrderType.Market;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
