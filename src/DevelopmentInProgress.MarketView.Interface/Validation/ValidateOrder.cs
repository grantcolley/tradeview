using System;
using DevelopmentInProgress.MarketView.Interface.Helpers;
using DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.MarketView.Interface.Validation
{
    public abstract class ValidateOrder : IValidateClientOrder
    {
        public void Validate(ClientOrder clientOrder)
        {
            string message = string.Empty;

            if(string.IsNullOrWhiteSpace(clientOrder.Symbol))
            {
                message = "No symbol;";
            }

            if(clientOrder.Quantity.Equals(0))
            {
                message += "Quantity cannot be 0;";
            }

            if(!string.IsNullOrWhiteSpace(message))
            {
                message = message.Insert(0, $"{OrderHelper.GetOrderTypeName(clientOrder.Type)} not valid: ");
                message = message.Remove(message.Length - 1, 1);
                throw new Exception(message);
            }
        }
    }
}