using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.Validation;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Extensions
{
    public static class SymbolExtensions
    {
        private static Dictionary<OrderType, IValidateClientOrder> orderValidation;

        static SymbolExtensions()
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

        public static void  ValidateClientOrder(this Symbol symbol, ClientOrder clientOrder)
        {
            string message;
            if(!orderValidation[clientOrder.Type].TryValidate(symbol, clientOrder, out message))
            {
                throw new OrderValidationException(message);
            }
        }
    }
}
