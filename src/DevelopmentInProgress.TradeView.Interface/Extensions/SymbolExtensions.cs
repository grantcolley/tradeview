using DevelopmentInProgress.TradeView.Interface.Model;
using DevelopmentInProgress.TradeView.Interface.Validation;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Extensions
{
    public static class SymbolExtensions
    {
        private static readonly Dictionary<OrderType, IValidateClientOrder> orderValidation = new Dictionary<OrderType, IValidateClientOrder>
        {
            { OrderType.Limit, new ValidateLimit()},
            { OrderType.LimitMaker, new ValidateLimit()},
            {OrderType.StopLossLimit, new ValidateStopOrderLimit() },
            {OrderType.TakeProfitLimit, new ValidateStopOrderLimit() },
            { OrderType.Market, new ValidateMarket()},
            { OrderType.StopLoss, new ValidateStopOrder()},
            {OrderType.TakeProfit, new ValidateStopOrder() }
        };

        public static void  ValidateClientOrder(this Symbol symbol, ClientOrder clientOrder)
        {
            if (clientOrder == null)
            {
                throw new ArgumentNullException(nameof(clientOrder));
            }

            if(!orderValidation[clientOrder.Type].TryValidate(symbol, clientOrder, out string message))
            {
                throw new OrderValidationException(message);
            }
        }
    }
}
