using Kucoin.Net.Objects;
using System;

namespace DevelopmentInProgress.TradeView.Api.Kucoin
{
    public static class CandlestickIntervalExtension
    {
        public static KucoinKlineInterval ToKucoinCandlestickInterval(this Interface.Model.CandlestickInterval candlestickInterval)
        {
            switch (candlestickInterval)
            {
                case Interface.Model.CandlestickInterval.Minute:
                    return KucoinKlineInterval.OneMinute;
                case Interface.Model.CandlestickInterval.Minutes_3:
                    return KucoinKlineInterval.ThreeMinutes;
                case Interface.Model.CandlestickInterval.Minutes_5:
                    return KucoinKlineInterval.FiveMinutes;
                case Interface.Model.CandlestickInterval.Minutes_15:
                    return KucoinKlineInterval.FifteenMinutes;
                case Interface.Model.CandlestickInterval.Minutes_30:
                    return KucoinKlineInterval.ThirtyMinutes;
                case Interface.Model.CandlestickInterval.Hour:
                    return KucoinKlineInterval.OneHour;
                case Interface.Model.CandlestickInterval.Hours_2:
                    return KucoinKlineInterval.TwoHours;
                case Interface.Model.CandlestickInterval.Hours_4:
                    return KucoinKlineInterval.FourHours;
                case Interface.Model.CandlestickInterval.Hours_6:
                    return KucoinKlineInterval.SixHours;
                case Interface.Model.CandlestickInterval.Hours_8:
                    return KucoinKlineInterval.EightHours;
                case Interface.Model.CandlestickInterval.Hours_12:
                    return KucoinKlineInterval.TwelfHours;
                case Interface.Model.CandlestickInterval.Day:
                    return KucoinKlineInterval.OneDay;
                case Interface.Model.CandlestickInterval.Week:
                    return KucoinKlineInterval.OneWeek;
                default:
                    throw new NotImplementedException();
            }
        }

        public static Interface.Model.CandlestickInterval ToMarketViewCandlestickInterval(this KucoinKlineInterval candlestickInterval)
        {
            switch (candlestickInterval)
            {
                case KucoinKlineInterval.OneMinute:
                    return Interface.Model.CandlestickInterval.Minute;
                case KucoinKlineInterval.ThreeMinutes:
                    return Interface.Model.CandlestickInterval.Minutes_3;
                case KucoinKlineInterval.FiveMinutes:
                    return Interface.Model.CandlestickInterval.Minutes_5;
                case KucoinKlineInterval.FifteenMinutes:
                    return Interface.Model.CandlestickInterval.Minutes_15;
                case KucoinKlineInterval.ThirtyMinutes:
                    return Interface.Model.CandlestickInterval.Minutes_30;
                case KucoinKlineInterval.OneHour:
                    return Interface.Model.CandlestickInterval.Hour;
                case KucoinKlineInterval.TwoHours:
                    return Interface.Model.CandlestickInterval.Hours_2;
                case KucoinKlineInterval.FourHours:
                    return Interface.Model.CandlestickInterval.Hours_4;
                case KucoinKlineInterval.SixHours:
                    return Interface.Model.CandlestickInterval.Hours_6;
                case KucoinKlineInterval.EightHours:
                    return Interface.Model.CandlestickInterval.Hours_8;
                case KucoinKlineInterval.TwelfHours:
                    return Interface.Model.CandlestickInterval.Hours_12;
                case KucoinKlineInterval.OneDay:
                    return Interface.Model.CandlestickInterval.Day;
                case KucoinKlineInterval.OneWeek:
                    return Interface.Model.CandlestickInterval.Week;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}