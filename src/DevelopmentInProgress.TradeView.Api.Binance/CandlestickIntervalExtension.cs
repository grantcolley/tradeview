using Binance;
using System;

namespace DevelopmentInProgress.TradeView.Api.Binance
{
    public static class CandlestickIntervalExtension
    {
        public static CandlestickInterval ToBinanceCandlestickInterval(this Core.Model.CandlestickInterval candlestickInterval)
        {
            switch (candlestickInterval)
            {
                case Core.Model.CandlestickInterval.Minute:
                    return CandlestickInterval.Minute;
                case Core.Model.CandlestickInterval.Minutes3:
                    return CandlestickInterval.Minutes_3;
                case Core.Model.CandlestickInterval.Minutes5:
                    return CandlestickInterval.Minutes_5;
                case Core.Model.CandlestickInterval.Minutes15:
                    return CandlestickInterval.Minutes_15;
                case Core.Model.CandlestickInterval.Minutes30:
                    return CandlestickInterval.Minutes_30;
                case Core.Model.CandlestickInterval.Hour:
                    return CandlestickInterval.Hour;
                case Core.Model.CandlestickInterval.Hours2:
                    return CandlestickInterval.Hours_2;
                case Core.Model.CandlestickInterval.Hours4:
                    return CandlestickInterval.Hours_4;
                case Core.Model.CandlestickInterval.Hours6:
                    return CandlestickInterval.Hours_6;
                case Core.Model.CandlestickInterval.Hours8:
                    return CandlestickInterval.Hours_8;
                case Core.Model.CandlestickInterval.Hours12:
                    return CandlestickInterval.Hours_12;
                case Core.Model.CandlestickInterval.Day:
                    return CandlestickInterval.Day;
                case Core.Model.CandlestickInterval.Days3:
                    return CandlestickInterval.Days_3;
                case Core.Model.CandlestickInterval.Week:
                    return CandlestickInterval.Week;
                case Core.Model.CandlestickInterval.Month:
                    return CandlestickInterval.Month;
                default:
                    throw new NotImplementedException();
            }
        }

        public static Core.Model.CandlestickInterval ToTradeViewCandlestickInterval(this CandlestickInterval candlestickInterval)
        {
            switch (candlestickInterval)
            {
                case CandlestickInterval.Minute:
                    return Core.Model.CandlestickInterval.Minute;
                case CandlestickInterval.Minutes_3:
                    return Core.Model.CandlestickInterval.Minutes3;
                case CandlestickInterval.Minutes_5:
                    return Core.Model.CandlestickInterval.Minutes5;
                case CandlestickInterval.Minutes_15:
                    return Core.Model.CandlestickInterval.Minutes15;
                case CandlestickInterval.Minutes_30:
                    return Core.Model.CandlestickInterval.Minutes30;
                case CandlestickInterval.Hour:
                    return Core.Model.CandlestickInterval.Hour;
                case CandlestickInterval.Hours_2:
                    return Core.Model.CandlestickInterval.Hours2;
                case CandlestickInterval.Hours_4:
                    return Core.Model.CandlestickInterval.Hours4;
                case CandlestickInterval.Hours_6:
                    return Core.Model.CandlestickInterval.Hours6;
                case CandlestickInterval.Hours_8:
                    return Core.Model.CandlestickInterval.Hours8;
                case CandlestickInterval.Hours_12:
                    return Core.Model.CandlestickInterval.Hours12;
                case CandlestickInterval.Day:
                    return Core.Model.CandlestickInterval.Day;
                case CandlestickInterval.Days_3:
                    return Core.Model.CandlestickInterval.Days3;
                case CandlestickInterval.Week:
                    return Core.Model.CandlestickInterval.Week;
                case CandlestickInterval.Month:
                    return Core.Model.CandlestickInterval.Month;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}