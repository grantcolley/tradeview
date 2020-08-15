using DevelopmentInProgress.TradeView.Core.Model;
using Kucoin.Net.Objects;
using System;

namespace DevelopmentInProgress.TradeView.Api.Kucoin
{
    public static class OrderTypeExtensions
    {
        public static KucoinNewOrderType ToKucoinNewOrderType(this OrderType order)
        {
            return order switch
            {
                OrderType.Limit => KucoinNewOrderType.Limit,
                OrderType.Market => KucoinNewOrderType.Market,
                _ => throw new NotImplementedException(),
            };
        }

        public static OrderType ToTradeViewOrderType(this KucoinOrderType order)
        {
            return order switch
            {
                KucoinOrderType.Limit => OrderType.Limit,
                KucoinOrderType.LimitStop => OrderType.StopLossLimit,
                KucoinOrderType.Market => OrderType.Market,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
