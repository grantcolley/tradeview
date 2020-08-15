using Binance;
using System;

namespace DevelopmentInProgress.TradeView.Api.Binance
{
    public static class CandlestickIntervalExtension
    {
        public static CandlestickInterval ToBinanceCandlestickInterval(this Core.Model.CandlestickInterval candlestickInterval)
        {
            return candlestickInterval switch
            {
                Core.Model.CandlestickInterval.Minute => CandlestickInterval.Minute,
                Core.Model.CandlestickInterval.Minutes3 => CandlestickInterval.Minutes_3,
                Core.Model.CandlestickInterval.Minutes5 => CandlestickInterval.Minutes_5,
                Core.Model.CandlestickInterval.Minutes15 => CandlestickInterval.Minutes_15,
                Core.Model.CandlestickInterval.Minutes30 => CandlestickInterval.Minutes_30,
                Core.Model.CandlestickInterval.Hour => CandlestickInterval.Hour,
                Core.Model.CandlestickInterval.Hours2 => CandlestickInterval.Hours_2,
                Core.Model.CandlestickInterval.Hours4 => CandlestickInterval.Hours_4,
                Core.Model.CandlestickInterval.Hours6 => CandlestickInterval.Hours_6,
                Core.Model.CandlestickInterval.Hours8 => CandlestickInterval.Hours_8,
                Core.Model.CandlestickInterval.Hours12 => CandlestickInterval.Hours_12,
                Core.Model.CandlestickInterval.Day => CandlestickInterval.Day,
                Core.Model.CandlestickInterval.Days3 => CandlestickInterval.Days_3,
                Core.Model.CandlestickInterval.Week => CandlestickInterval.Week,
                Core.Model.CandlestickInterval.Month => CandlestickInterval.Month,
                _ => throw new NotImplementedException(),
            };
        }

        public static Core.Model.CandlestickInterval ToTradeViewCandlestickInterval(this CandlestickInterval candlestickInterval)
        {
            return candlestickInterval switch
            {
                CandlestickInterval.Minute => Core.Model.CandlestickInterval.Minute,
                CandlestickInterval.Minutes_3 => Core.Model.CandlestickInterval.Minutes3,
                CandlestickInterval.Minutes_5 => Core.Model.CandlestickInterval.Minutes5,
                CandlestickInterval.Minutes_15 => Core.Model.CandlestickInterval.Minutes15,
                CandlestickInterval.Minutes_30 => Core.Model.CandlestickInterval.Minutes30,
                CandlestickInterval.Hour => Core.Model.CandlestickInterval.Hour,
                CandlestickInterval.Hours_2 => Core.Model.CandlestickInterval.Hours2,
                CandlestickInterval.Hours_4 => Core.Model.CandlestickInterval.Hours4,
                CandlestickInterval.Hours_6 => Core.Model.CandlestickInterval.Hours6,
                CandlestickInterval.Hours_8 => Core.Model.CandlestickInterval.Hours8,
                CandlestickInterval.Hours_12 => Core.Model.CandlestickInterval.Hours12,
                CandlestickInterval.Day => Core.Model.CandlestickInterval.Day,
                CandlestickInterval.Days_3 => Core.Model.CandlestickInterval.Days3,
                CandlestickInterval.Week => Core.Model.CandlestickInterval.Week,
                CandlestickInterval.Month => Core.Model.CandlestickInterval.Month,
                _ => throw new NotImplementedException(),
            };
        }
    }
}