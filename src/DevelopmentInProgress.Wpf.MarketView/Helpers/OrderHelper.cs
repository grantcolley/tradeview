using DevelopmentInProgress.MarketView.Interface.Model;
using System;
using System.Text.RegularExpressions;

namespace DevelopmentInProgress.Wpf.MarketView.Helpers
{
    public static class OrderHelper
    {
        public static string GetOrderStatusName(OrderStatus orderStatus)
        {
            var result = Enum.GetName(typeof(OrderStatus), orderStatus);
            return Regex.Replace(result, "[A-Z]", " $0").Trim();
        }

        public static string GetTimeInForceName(TimeInForce timeInForce)
        {
            var result = Enum.GetName(typeof(TimeInForce), timeInForce);
            return Regex.Replace(result, "[A-Z]", " $0").Trim();
        }

        public static string GetOrderSideName(OrderSide orderSide)
        {
            var result = Enum.GetName(typeof(OrderSide), orderSide);
            return Regex.Replace(result, "[A-Z]", " $0").Trim();
        }        
    }
}
