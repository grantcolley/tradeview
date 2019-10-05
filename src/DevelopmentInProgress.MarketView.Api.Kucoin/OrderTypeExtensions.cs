using DevelopmentInProgress.MarketView.Interface.Model;
using Kucoin.Net.Objects;
using System;

namespace DevelopmentInProgress.MarketView.Api.Kucoin
{
    public static class OrderTypeExtensions
    {
        public static KucoinOrderType ToKucoinOrderType(this OrderType order)
        {
            switch (order)
            {
                case OrderType.Limit:
                    return KucoinOrderType.Limit;
                case OrderType.StopLossLimit:
                    return KucoinOrderType.LimitStop;
                case OrderType.Market:
                    return KucoinOrderType.Market;
                default:
                    throw new NotImplementedException();
            }
        }

        public static OrderType ToMarketViewOrderType(this KucoinOrderType order)
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
