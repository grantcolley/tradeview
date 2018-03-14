using DevelopmentInProgress.MarketView.Interface.Model;
using System;
using System.Text.RegularExpressions;

namespace DevelopmentInProgress.MarketView.Interface.Helpers
{
    public static class OrderTypeHelper
    {
        public static string[] OrderTypes()
        {
            var source = Enum.GetNames(typeof(OrderType));
            var list = new string[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                list[i] = Regex.Replace(source[i], "[A-Z]", " $0").Trim();
            }

            return list;
        }
    }
}
