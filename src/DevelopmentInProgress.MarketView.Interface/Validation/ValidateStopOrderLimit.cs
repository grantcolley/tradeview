using System;
using DevelopmentInProgress.MarketView.Interface.Helpers;
using DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.MarketView.Interface.Validation
{
    public class ValidateStopOrderLimit : ValidateOrder, IValidateClientOrder
    {
        public new void Validate(Symbol symbol, ClientOrder clientOrder)
        {
            base.Validate(symbol, clientOrder);

            string message = string.Empty;

            if (clientOrder.Price.Equals(0))
            {
                message += "Price cannot be 0;";
            }

            if (clientOrder.StopPrice.Equals(0))
            {
                message += "Stop price cannot be 0;";
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                message = message.Insert(0, $"{OrderHelper.GetOrderTypeName(clientOrder.Type)} order not valid: ");
                message = message.Remove(message.Length - 1, 1);
                throw new Exception(message);
            }
        }
    }
}
