using DevelopmentInProgress.TradeView.Core.Model;

namespace DevelopmentInProgress.TradeView.Api.BinanceNet
{
    public static class TimeInForceExtensions
    {
        public static Binance.Net.Enums.TimeInForce ToBinanceTimeInForce(this TimeInForce tif)
        {
            return tif switch
            {
                TimeInForce.FOK => Binance.Net.Enums.TimeInForce.FillOrKill,
                TimeInForce.GTC => Binance.Net.Enums.TimeInForce.GoodTillCanceled,
                TimeInForce.IOC => Binance.Net.Enums.TimeInForce.ImmediateOrCancel,
                _ => throw new NotImplementedException(),
            };
        }

        public static TimeInForce ToTradeViewTimeInForce(this Binance.Net.Enums.TimeInForce tif)
        {
            return tif switch
            {
                Binance.Net.Enums.TimeInForce.FillOrKill => TimeInForce.FOK,
                Binance.Net.Enums.TimeInForce.GoodTillCanceled => TimeInForce.GTC,
                Binance.Net.Enums.TimeInForce.ImmediateOrCancel => TimeInForce.IOC,
                _ => throw new NotImplementedException(),
            };
        }
    }
}