using DevelopmentInProgress.TradeView.Core.Enums;
using System;
using System.Text.RegularExpressions;

namespace DevelopmentInProgress.TradeView.Core.Extensions
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
            if(exchange == null)
            {
                throw new ArgumentNullException(nameof(exchange));
            }

            return (Exchange)Enum.Parse(typeof(Exchange), exchange.Replace(" ", "", StringComparison.Ordinal));
        }
    }
}
