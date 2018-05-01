using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.Validation;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DevelopmentInProgress.MarketView.Interface.Helpers
{
    public static class OrderHelper
    {
        private static Dictionary<OrderType, IValidateClientOrder> orderValidation;

        static OrderHelper()
        {
            orderValidation = new Dictionary<OrderType, IValidateClientOrder>();
            orderValidation.Add(OrderType.Limit, new ValidateLimit());
            orderValidation.Add(OrderType.LimitMaker, new ValidateLimit());
            orderValidation.Add(OrderType.StopLossLimit, new ValidateStopOrderLimit());
            orderValidation.Add(OrderType.TakeProfitLimit, new ValidateStopOrderLimit());
            orderValidation.Add(OrderType.Market, new ValidateMarket());
            orderValidation.Add(OrderType.StopLoss, new ValidateStopOrder());
            orderValidation.Add(OrderType.TakeProfit, new ValidateStopOrder());
        }

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
            if (string.IsNullOrWhiteSpace(compare))
            {
                return false;
            }

            OrderType result;
            if (Enum.TryParse<OrderType>(compare.Replace(" ", ""), out result))
            {
                return orderType.Equals(result);
            }

            return false;
        }

        public static bool IsMarketOrder(string orderType)
        {
            if (string.IsNullOrEmpty(orderType))
            {
                return false;
            }

            OrderType result;
            if (Enum.TryParse<OrderType>(orderType.Replace(" ", ""), out result))
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

        public static bool IsStopLoss(string orderType)
        {
            if (string.IsNullOrEmpty(orderType))
            {
                return false;
            }

            OrderType result;
            if (Enum.TryParse<OrderType>(orderType.Replace(" ", ""), out result))
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

        public static OrderType GetOrderType(string orderType)
        {
            return (OrderType)Enum.Parse(typeof(OrderType), orderType.Replace(" ", ""));
        }

        public static string GetOrderTypeName(OrderType orderType)
        {
            var result = Enum.GetName(typeof(OrderType), orderType);
            return Regex.Replace(result, "[A-Z]", " $0").Trim();
        }

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

        public static void ValidateClientOrder(Symbol symbol, ClientOrder clientOrder)
        {
            orderValidation[clientOrder.Type].Validate(symbol, clientOrder);
        }
    }
}
