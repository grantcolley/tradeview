using DevelopmentInProgress.TradeView.Core.Model;
using Kucoin.Net.Objects;
using System;

namespace DevelopmentInProgress.TradeView.Api.Kucoin
{
    public static class CandlestickIntervalExtension
    {
        public static KucoinKlineInterval ToKucoinCandlestickInterval(this CandlestickInterval candlestickInterval)
        {
            return candlestickInterval switch
            {
                CandlestickInterval.Minute => KucoinKlineInterval.OneMinute,
                CandlestickInterval.Minutes3 => KucoinKlineInterval.ThreeMinutes,
                CandlestickInterval.Minutes5 => KucoinKlineInterval.FiveMinutes,
                CandlestickInterval.Minutes15 => KucoinKlineInterval.FifteenMinutes,
                CandlestickInterval.Minutes30 => KucoinKlineInterval.ThirtyMinutes,
                CandlestickInterval.Hour => KucoinKlineInterval.OneHour,
                CandlestickInterval.Hours2 => KucoinKlineInterval.TwoHours,
                CandlestickInterval.Hours4 => KucoinKlineInterval.FourHours,
                CandlestickInterval.Hours6 => KucoinKlineInterval.SixHours,
                CandlestickInterval.Hours8 => KucoinKlineInterval.EightHours,
                CandlestickInterval.Hours12 => KucoinKlineInterval.TwelfHours,
                CandlestickInterval.Day => KucoinKlineInterval.OneDay,
                CandlestickInterval.Week => KucoinKlineInterval.OneWeek,
                _ => throw new NotImplementedException(),
            };
        }

        public static CandlestickInterval ToTradeViewCandlestickInterval(this KucoinKlineInterval candlestickInterval)
        {
            return candlestickInterval switch
            {
                KucoinKlineInterval.OneMinute => CandlestickInterval.Minute,
                KucoinKlineInterval.ThreeMinutes => CandlestickInterval.Minutes3,
                KucoinKlineInterval.FiveMinutes => CandlestickInterval.Minutes5,
                KucoinKlineInterval.FifteenMinutes => CandlestickInterval.Minutes15,
                KucoinKlineInterval.ThirtyMinutes => CandlestickInterval.Minutes30,
                KucoinKlineInterval.OneHour => CandlestickInterval.Hour,
                KucoinKlineInterval.TwoHours => CandlestickInterval.Hours2,
                KucoinKlineInterval.FourHours => CandlestickInterval.Hours4,
                KucoinKlineInterval.SixHours => CandlestickInterval.Hours6,
                KucoinKlineInterval.EightHours => CandlestickInterval.Hours8,
                KucoinKlineInterval.TwelfHours => CandlestickInterval.Hours12,
                KucoinKlineInterval.OneDay => CandlestickInterval.Day,
                KucoinKlineInterval.OneWeek => CandlestickInterval.Week,
                _ => throw new NotImplementedException(),
            };
        }
    }
}