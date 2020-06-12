using DevelopmentInProgress.TradeView.Core.Model;
using Kucoin.Net.Objects;
using System;

namespace DevelopmentInProgress.TradeView.Api.Kucoin
{
    public static class TimeInForceExtensions
    {
        public static KucoinTimeInForce ToKucoinTimeInForce(this TimeInForce tif)
        {
            switch (tif)
            {
                case TimeInForce.FOK:
                    return KucoinTimeInForce.FillOrKill;
                case TimeInForce.GTC:
                    return KucoinTimeInForce.GoodTillCancelled;
                case TimeInForce.IOC:
                    return KucoinTimeInForce.ImmediateOrCancel;
                default:
                    throw new NotImplementedException();
            }
        }

        public static TimeInForce ToTradeViewTimeInForce(this KucoinTimeInForce tif)
        {
            switch (tif)
            {
                case KucoinTimeInForce.FillOrKill:
                    return TimeInForce.FOK;
                case KucoinTimeInForce.GoodTillCancelled:
                    return TimeInForce.GTC;
                case KucoinTimeInForce.ImmediateOrCancel:
                    return TimeInForce.IOC;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}