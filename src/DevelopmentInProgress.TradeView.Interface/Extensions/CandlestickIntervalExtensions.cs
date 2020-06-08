using DevelopmentInProgress.TradeView.Interface.Model;
using System;

namespace DevelopmentInProgress.TradeView.Interface.Extensions
{
    public static class CandlestickIntervalExtensions
    {
        public static string[] GetCandlestickIntervalNames()
        {
            return Enum.GetNames(typeof(CandlestickInterval));
        }

        public static CandlestickInterval GetCandlestickInterval(this string candlestickInterval)
        {
            if(string.IsNullOrWhiteSpace(candlestickInterval))
            {
                return CandlestickInterval.Day;
            }

            return (CandlestickInterval)Enum.Parse(typeof(CandlestickInterval), candlestickInterval);
        }
    }
}
