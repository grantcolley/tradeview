using Binance.Net.Enums;
using DevelopmentInProgress.TradeView.Core.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Api.BinanceNet
{
    public static class OrderTypeExtensions
    {
        public static SpotOrderType ToSpotOrderType(this OrderType order)
        {
            return order switch
            {
                OrderType.Limit => SpotOrderType.Limit,
                OrderType.Market => SpotOrderType.Market,
                OrderType.StopLoss => SpotOrderType.StopLoss,
                OrderType.StopLossLimit => SpotOrderType.StopLossLimit,
                OrderType.TakeProfit => SpotOrderType.TakeProfit,
                OrderType.TakeProfitLimit => SpotOrderType.TakeProfitLimit,
                OrderType.LimitMaker => SpotOrderType.LimitMaker,
                _ => throw new NotImplementedException(),
            };
        }

        public static OrderType ToTradeViewOrderType(this SpotOrderType order)
        {
            return order switch
            {
                SpotOrderType.Limit => OrderType.Limit,
                SpotOrderType.Market => OrderType.Market,
                SpotOrderType.StopLoss => OrderType.StopLoss,
                SpotOrderType.StopLossLimit => OrderType.StopLossLimit,
                SpotOrderType.TakeProfit => OrderType.TakeProfit,
                SpotOrderType.TakeProfitLimit => OrderType.TakeProfitLimit,
                SpotOrderType.LimitMaker => OrderType.LimitMaker,
                _ => throw new NotImplementedException(),
            };
        }
    }
}