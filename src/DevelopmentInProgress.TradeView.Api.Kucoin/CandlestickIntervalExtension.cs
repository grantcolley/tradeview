using DevelopmentInProgress.TradeView.Core.Model;
using Kucoin.Net.Objects;
using System;

namespace DevelopmentInProgress.TradeView.Api.Kucoin
{
    public static class CandlestickIntervalExtension
    {
        public static KucoinKlineInterval ToKucoinCandlestickInterval(this CandlestickInterval candlestickInterval)
        {
            switch (candlestickInterval)
            {
                case CandlestickInterval.Minute:
                    return KucoinKlineInterval.OneMinute;
                case CandlestickInterval.Minutes3:
                    return KucoinKlineInterval.ThreeMinutes;
                case CandlestickInterval.Minutes5:
                    return KucoinKlineInterval.FiveMinutes;
                case CandlestickInterval.Minutes15:
                    return KucoinKlineInterval.FifteenMinutes;
                case CandlestickInterval.Minutes30:
                    return KucoinKlineInterval.ThirtyMinutes;
                case CandlestickInterval.Hour:
                    return KucoinKlineInterval.OneHour;
                case CandlestickInterval.Hours2:
                    return KucoinKlineInterval.TwoHours;
                case CandlestickInterval.Hours4:
                    return KucoinKlineInterval.FourHours;
                case CandlestickInterval.Hours6:
                    return KucoinKlineInterval.SixHours;
                case CandlestickInterval.Hours8:
                    return KucoinKlineInterval.EightHours;
                case CandlestickInterval.Hours12:
                    return KucoinKlineInterval.TwelfHours;
                case CandlestickInterval.Day:
                    return KucoinKlineInterval.OneDay;
                case CandlestickInterval.Week:
                    return KucoinKlineInterval.OneWeek;
                default:
                    throw new NotImplementedException();
            }
        }

        public static CandlestickInterval ToTradeViewCandlestickInterval(this KucoinKlineInterval candlestickInterval)
        {
            switch (candlestickInterval)
            {
                case KucoinKlineInterval.OneMinute:
                    return CandlestickInterval.Minute;
                case KucoinKlineInterval.ThreeMinutes:
                    return CandlestickInterval.Minutes3;
                case KucoinKlineInterval.FiveMinutes:
                    return CandlestickInterval.Minutes5;
                case KucoinKlineInterval.FifteenMinutes:
                    return CandlestickInterval.Minutes15;
                case KucoinKlineInterval.ThirtyMinutes:
                    return CandlestickInterval.Minutes30;
                case KucoinKlineInterval.OneHour:
                    return CandlestickInterval.Hour;
                case KucoinKlineInterval.TwoHours:
                    return CandlestickInterval.Hours2;
                case KucoinKlineInterval.FourHours:
                    return CandlestickInterval.Hours4;
                case KucoinKlineInterval.SixHours:
                    return CandlestickInterval.Hours6;
                case KucoinKlineInterval.EightHours:
                    return CandlestickInterval.Hours8;
                case KucoinKlineInterval.TwelfHours:
                    return CandlestickInterval.Hours12;
                case KucoinKlineInterval.OneDay:
                    return CandlestickInterval.Day;
                case KucoinKlineInterval.OneWeek:
                    return CandlestickInterval.Week;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}