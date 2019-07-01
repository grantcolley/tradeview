using DevelopmentInProgress.MarketView.Interface.Model;
using System;

namespace DevelopmentInProgress.MarketView.Interface.Extensions
{
    public static class CandlestickIntervalExtensions
    {
        public static string[] GetCandlestickIntervalNames()
        {
            var source = Enum.GetNames(typeof(CandlestickInterval));
            var list = new string[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                list[i] = source[i].Replace("_", " ").Trim();
            }

            return list;
        }

        public static CandlestickInterval GetCandlestickInterval(this string candlestickInterval)
        {
            if(string.IsNullOrWhiteSpace(candlestickInterval))
            {
                return CandlestickInterval.Day;
            }

            return (CandlestickInterval)Enum.Parse(typeof(CandlestickInterval), candlestickInterval.Replace(" ", "_"));
        }
    }
}
