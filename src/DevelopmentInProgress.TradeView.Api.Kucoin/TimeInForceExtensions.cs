using DevelopmentInProgress.TradeView.Core.Model;
using Kucoin.Net.Objects;
using System;

namespace DevelopmentInProgress.TradeView.Api.Kucoin
{
    public static class TimeInForceExtensions
    {
        public static KucoinTimeInForce ToKucoinTimeInForce(this TimeInForce tif)
        {
            return tif switch
            {
                TimeInForce.FOK => KucoinTimeInForce.FillOrKill,
                TimeInForce.GTC => KucoinTimeInForce.GoodTillCancelled,
                TimeInForce.IOC => KucoinTimeInForce.ImmediateOrCancel,
                _ => throw new NotImplementedException(),
            };
        }

        public static TimeInForce ToTradeViewTimeInForce(this KucoinTimeInForce tif)
        {
            return tif switch
            {
                KucoinTimeInForce.FillOrKill => TimeInForce.FOK,
                KucoinTimeInForce.GoodTillCancelled => TimeInForce.GTC,
                KucoinTimeInForce.ImmediateOrCancel => TimeInForce.IOC,
                _ => throw new NotImplementedException(),
            };
        }
    }
}