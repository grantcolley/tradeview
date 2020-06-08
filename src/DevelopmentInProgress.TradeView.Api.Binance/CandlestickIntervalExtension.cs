using Binance;
using System;

namespace DevelopmentInProgress.TradeView.Api.Binance
{
    public static class CandlestickIntervalExtension
    {
        public static CandlestickInterval ToBinanceCandlestickInterval(this Interface.Model.CandlestickInterval candlestickInterval)
        {
            switch (candlestickInterval)
            {
                case Interface.Model.CandlestickInterval.Minute:
                    return CandlestickInterval.Minute;
                case Interface.Model.CandlestickInterval.Minutes3:
                    return CandlestickInterval.Minutes_3;
                case Interface.Model.CandlestickInterval.Minutes5:
                    return CandlestickInterval.Minutes_5;
                case Interface.Model.CandlestickInterval.Minutes15:
                    return CandlestickInterval.Minutes_15;
                case Interface.Model.CandlestickInterval.Minutes30:
                    return CandlestickInterval.Minutes_30;
                case Interface.Model.CandlestickInterval.Hour:
                    return CandlestickInterval.Hour;
                case Interface.Model.CandlestickInterval.Hours2:
                    return CandlestickInterval.Hours_2;
                case Interface.Model.CandlestickInterval.Hours4:
                    return CandlestickInterval.Hours_4;
                case Interface.Model.CandlestickInterval.Hours6:
                    return CandlestickInterval.Hours_6;
                case Interface.Model.CandlestickInterval.Hours8:
                    return CandlestickInterval.Hours_8;
                case Interface.Model.CandlestickInterval.Hours12:
                    return CandlestickInterval.Hours_12;
                case Interface.Model.CandlestickInterval.Day:
                    return CandlestickInterval.Day;
                case Interface.Model.CandlestickInterval.Days3:
                    return CandlestickInterval.Days_3;
                case Interface.Model.CandlestickInterval.Week:
                    return CandlestickInterval.Week;
                case Interface.Model.CandlestickInterval.Month:
                    return CandlestickInterval.Month;
                default:
                    throw new NotImplementedException();
            }
        }

        public static Interface.Model.CandlestickInterval ToTradeViewCandlestickInterval(this CandlestickInterval candlestickInterval)
        {
            switch (candlestickInterval)
            {
                case CandlestickInterval.Minute:
                    return Interface.Model.CandlestickInterval.Minute;
                case CandlestickInterval.Minutes_3:
                    return Interface.Model.CandlestickInterval.Minutes3;
                case CandlestickInterval.Minutes_5:
                    return Interface.Model.CandlestickInterval.Minutes5;
                case CandlestickInterval.Minutes_15:
                    return Interface.Model.CandlestickInterval.Minutes15;
                case CandlestickInterval.Minutes_30:
                    return Interface.Model.CandlestickInterval.Minutes30;
                case CandlestickInterval.Hour:
                    return Interface.Model.CandlestickInterval.Hour;
                case CandlestickInterval.Hours_2:
                    return Interface.Model.CandlestickInterval.Hours2;
                case CandlestickInterval.Hours_4:
                    return Interface.Model.CandlestickInterval.Hours4;
                case CandlestickInterval.Hours_6:
                    return Interface.Model.CandlestickInterval.Hours6;
                case CandlestickInterval.Hours_8:
                    return Interface.Model.CandlestickInterval.Hours8;
                case CandlestickInterval.Hours_12:
                    return Interface.Model.CandlestickInterval.Hours12;
                case CandlestickInterval.Day:
                    return Interface.Model.CandlestickInterval.Day;
                case CandlestickInterval.Days_3:
                    return Interface.Model.CandlestickInterval.Days3;
                case CandlestickInterval.Week:
                    return Interface.Model.CandlestickInterval.Week;
                case CandlestickInterval.Month:
                    return Interface.Model.CandlestickInterval.Month;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}