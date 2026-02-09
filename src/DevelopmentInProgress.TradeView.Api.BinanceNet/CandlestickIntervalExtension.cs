using Binance.Net.Enums;
using DevelopmentInProgress.TradeView.Core.Model;

namespace DevelopmentInProgress.TradeView.Api.BinanceNet
{
    public static class CandlestickIntervalExtension
    {
        public static KlineInterval ToKlineInterval(this CandlestickInterval candlestickInterval)
        {
            return candlestickInterval switch
            {
                CandlestickInterval.Minute => KlineInterval.OneMinute,
                CandlestickInterval.Minutes3 => KlineInterval.ThreeMinutes,
                CandlestickInterval.Minutes5 => KlineInterval.FiveMinutes,
                CandlestickInterval.Minutes15 => KlineInterval.FifteenMinutes,
                CandlestickInterval.Minutes30 => KlineInterval.ThirtyMinutes,
                CandlestickInterval.Hour => KlineInterval.OneHour,
                CandlestickInterval.Hours2 => KlineInterval.TwoHour,
                CandlestickInterval.Hours4 => KlineInterval.FourHour,
                CandlestickInterval.Hours6 => KlineInterval.SixHour,
                CandlestickInterval.Hours8 => KlineInterval.EightHour,
                CandlestickInterval.Hours12 => KlineInterval.TwelveHour,
                CandlestickInterval.Day => KlineInterval.OneDay,
                CandlestickInterval.Week => KlineInterval.OneWeek,
                _ => throw new NotImplementedException(),
            };
        }

        public static CandlestickInterval ToTradeViewCandlestickInterval(this KlineInterval candlestickInterval)
        {
            return candlestickInterval switch
            {
                KlineInterval.OneMinute => CandlestickInterval.Minute,
                KlineInterval.ThreeMinutes => CandlestickInterval.Minutes3,
                KlineInterval.FiveMinutes => CandlestickInterval.Minutes5,
                KlineInterval.FifteenMinutes => CandlestickInterval.Minutes15,
                KlineInterval.ThirtyMinutes => CandlestickInterval.Minutes30,
                KlineInterval.OneHour => CandlestickInterval.Hour,
                KlineInterval.TwoHour => CandlestickInterval.Hours2,
                KlineInterval.FourHour => CandlestickInterval.Hours4,
                KlineInterval.SixHour => CandlestickInterval.Hours6,
                KlineInterval.EightHour => CandlestickInterval.Hours8,
                KlineInterval.TwelveHour => CandlestickInterval.Hours12,
                KlineInterval.OneDay => CandlestickInterval.Day,
                KlineInterval.OneWeek => CandlestickInterval.Week,
                _ => throw new NotImplementedException(),
            };
        }
    }
}