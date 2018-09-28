using DevelopmentInProgress.MarketView.Interface.Strategy;
using System;
using System.Text.RegularExpressions;

namespace DevelopmentInProgress.MarketView.Interface.Extensions
{
    public static class ExchangeExtensions
    {
        public static string[] Exchanges()
        {
            var source = Enum.GetNames(typeof(Exchange));
            var list = new string[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                list[i] = Regex.Replace(source[i], "[A-Z]", " $0").Trim();
            }

            return list;
        }

        public static Exchange GetExchange(this string exchange)
        {
            return (Exchange)Enum.Parse(typeof(Exchange), exchange.Replace(" ", ""));
        }
    }
}
