using DevelopmentInProgress.MarketView.Interface.Model;
using System;
using System.Text.RegularExpressions;

namespace DevelopmentInProgress.Wpf.MarketView.Helpers
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

        public static bool AreEqual(OrderType orderType, string compare)
        {
            if(string.IsNullOrWhiteSpace(compare))
            {
                return false;
            }

            OrderType result;
            if(Enum.TryParse<OrderType>(compare.Replace(" ", ""), out result))
            {
                return orderType.Equals(result);
            }

            return false;
        }

        public static string GetOrderTypeName(OrderType orderType)
        {
            var result = Enum.GetName(typeof(OrderType), orderType);
            return Regex.Replace(result, "[A-Z]", " $0").Trim();
        }
    }
}