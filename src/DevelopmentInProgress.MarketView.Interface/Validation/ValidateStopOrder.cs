using System;
using DevelopmentInProgress.MarketView.Interface.Helpers;
using DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.MarketView.Interface.Validation
{
    public class ValidateStopOrder : ValidateOrder, IValidateClientOrder
    {
        public new void Validate(Symbol symbol, ClientOrder clientOrder)
        {
            base.Validate(symbol, clientOrder);

            string message = string.Empty;

            if (clientOrder.StopPrice.Equals(0))
            {
                message = $"Stop price cannot be 0";
            }

            if (clientOrder.StopPrice < symbol.Price.Minimum)
            {
                message += $"Stop price {clientOrder.StopPrice} cannot be below minimum {symbol.Price.Minimum};";
            }

            if (clientOrder.StopPrice > symbol.Price.Maximum)
            {
                message += $"Stop price {clientOrder.StopPrice} cannot be above maximum {symbol.Price.Maximum};";
            }

            if ((clientOrder.StopPrice - symbol.Price.Minimum) % symbol.Price.Increment != 0)
            {
                message += $"Stop price {clientOrder.StopPrice} doesn't meet tick size {symbol.Price.Increment};";
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                message = message.Insert(0, $"{clientOrder.Symbol} {OrderHelper.GetOrderTypeName(clientOrder.Type)} not valid: ");
                message = message.Remove(message.Length - 1, 1);
                throw new Exception(message);
            }
        }
    }
}
