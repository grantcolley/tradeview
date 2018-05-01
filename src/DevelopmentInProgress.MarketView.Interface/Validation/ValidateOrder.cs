using System;
using DevelopmentInProgress.MarketView.Interface.Helpers;
using DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.MarketView.Interface.Validation
{
    public abstract class ValidateOrder : IValidateClientOrder
    {
        public void Validate(Symbol symbol, ClientOrder clientOrder)
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

            if (clientOrder.Quantity < symbol.Quantity.Minimum)
            {
                message += $"Quantity {clientOrder.Quantity} cannot be below minimum {symbol.Quantity.Minimum};";
            }

            if (clientOrder.Quantity > symbol.Quantity.Maximum)
            {
                message += $"Quantity {clientOrder.Quantity} cannot be above maximum {symbol.Quantity.Maximum};";
            }

            if ((clientOrder.Quantity - symbol.Quantity.Minimum) % symbol.Quantity.Increment != 0)
            {
                message += $"Quantity {clientOrder.Quantity} doesn't meet step size {symbol.Quantity.Increment};";
            }

            if(clientOrder.IcebergQuantity != 0)
            {
                if (clientOrder.IcebergQuantity < symbol.Quantity.Minimum)
                {
                    message += $"Iceberg Quantity {clientOrder.IcebergQuantity} cannot be below minimum {symbol.Quantity.Minimum};";
                }

                if (clientOrder.IcebergQuantity > symbol.Quantity.Maximum)
                {
                    message += $"Iceberg Quantity {clientOrder.IcebergQuantity} cannot be above maximum {symbol.Quantity.Maximum};";
                }

                if ((clientOrder.IcebergQuantity - symbol.Quantity.Minimum) % symbol.Quantity.Increment != 0)
                {
                    message += $"Iceberg Quantity {clientOrder.IcebergQuantity} doesn't meet step size {symbol.Quantity.Increment};";
                }
            }

            var notional = clientOrder.Price * clientOrder.Quantity;
            if (notional > symbol.NotionalMinimumValue)
            {
                message += $"Notional {notional} is greater then the minimum notional";
            }

            //if (clientOrder.Price < symbol.Price.Minimum)
            //{
            //    message += $"Price {clientOrder.Price} cannot be below minimum {symbol.Price.Minimum};";
            //}

            //if (clientOrder.Price > symbol.Price.Maximum)
            //{
            //    message += $"Price {clientOrder.Price} cannot be above maximum {symbol.Price.Maximum};";
            //}

            //if ((clientOrder.Price - symbol.Price.Minimum) % symbol.Price.Increment != 0)
            //{
            //    message += $"Price {clientOrder.Price} doesn't meet tick size {symbol.Price.Increment};";
            //}
            
            if (!string.IsNullOrWhiteSpace(message))
            {
                message = message.Insert(0, $"{clientOrder.Symbol} {OrderHelper.GetOrderTypeName(clientOrder.Type)} not valid: ");
                message = message.Remove(message.Length - 1, 1);
                throw new Exception(message);
            }
        }
    }
}