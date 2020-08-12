using DevelopmentInProgress.TradeView.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DevelopmentInProgress.TradeView.Core.Extensions
{
    public static class OrderExtensions
    {
        public static List<string> OrderTypes()
        {
            var source = Enum.GetNames(typeof(OrderType));
            var list = new string[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                list[i] = Regex.Replace(source[i], "[A-Z]", " $0").Trim();
            }

            return list.ToList();
        }

        public static List<string> GetOrderTypeNames(this IEnumerable<OrderType> orderTypes)
        {
            var range = orderTypes.Select(ot => ot.GetOrderTypeName());
            var list = new List<string>(range);
            return list;
        }

        public static bool AreEqual(this OrderType orderType, string compare)
        {
            if (string.IsNullOrWhiteSpace(compare))
            {
                return false;
            }

            if (Enum.TryParse<OrderType>(compare.Replace(" ", "", StringComparison.Ordinal), out OrderType result))
            {
                return orderType.Equals(result);
            }

            return false;
        }

        public static bool IsMarketOrder(this string orderType)
        {
            if (string.IsNullOrEmpty(orderType))
            {
                return false;
            }

            if (Enum.TryParse<OrderType>(orderType.Replace(" ", "", StringComparison.Ordinal), out OrderType result))
            {
                switch (result)
                {
                    case OrderType.Market:
                    case OrderType.StopLoss:
                    case OrderType.TakeProfit:
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

        public static bool IsStopLoss(this string orderType)
        {
            if (string.IsNullOrEmpty(orderType))
            {
                return false;
            }

            if (Enum.TryParse<OrderType>(orderType.Replace(" ", "", StringComparison.Ordinal), out OrderType result))
            {
                switch (result)
                {
                    case OrderType.StopLoss:
                    case OrderType.StopLossLimit:
                    case OrderType.TakeProfit:
                    case OrderType.TakeProfitLimit:
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

        public static OrderType GetOrderType(this string orderType)
        {
            if(orderType == null)
            {
                throw new ArgumentNullException(nameof(orderType));
            }

            return (OrderType)Enum.Parse(typeof(OrderType), orderType.Replace(" ", "", StringComparison.Ordinal));
        }

        public static string GetOrderTypeName(this OrderType orderType)
        {
            var result = Enum.GetName(typeof(OrderType), orderType);
            return Regex.Replace(result, "[A-Z]", " $0").Trim();
        }

        public static string GetOrderStatusName(this OrderStatus orderStatus)
        {
            var result = Enum.GetName(typeof(OrderStatus), orderStatus);
            return Regex.Replace(result, "[A-Z]", " $0").Trim();
        }

        public static string GetTimeInForceName(this TimeInForce timeInForce)
        {
            var result = Enum.GetName(typeof(TimeInForce), timeInForce);
            return Regex.Replace(result, "[A-Z]", " $0").Trim();
        }

        public static string GetOrderSideName(this OrderSide orderSide)
        {
            var result = Enum.GetName(typeof(OrderSide), orderSide);
            return Regex.Replace(result, "[A-Z]", " $0").Trim();
        }
    }
}
