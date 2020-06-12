using DevelopmentInProgress.TradeView.Core.Model;
using System;

namespace DevelopmentInProgress.TradeView.Core.Extensions
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
