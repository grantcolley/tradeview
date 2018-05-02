using DevelopmentInProgress.MarketView.Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevelopmentInProgress.MarketView.Interface.Validation
{
    public class ClientOrderValidationBuilder
    {
        private List<Action<Symbol, ClientOrder, StringBuilder>> validations;

        public ClientOrderValidationBuilder()
        {
            validations = new List<Action<Symbol, ClientOrder, StringBuilder>>();

            validations.Add((s, o, sb) =>
            {
                if (string.IsNullOrWhiteSpace(o.Symbol))
                {
                    sb.Append("No symbol;");
                }

                if(!s.OrderTypes.Contains(o.Type))
                {
                    sb.Append($"Order type {o.Type} is not permissted for this symbol");
                }

                var notional = o.Price * o.Quantity;
                if (notional > s.NotionalMinimumValue)
                {
                    sb.Append($"Notional {notional} is greater then the minimum notional");
                }

                if (o.Quantity.Equals(0))
                {
                    sb.Append("Quantity cannot be 0;");
                }

                if (o.Quantity < s.Quantity.Minimum)
                {
                    sb.Append($"Quantity {o.Quantity} cannot be below minimum {s.Quantity.Minimum};");
                }

                if (o.Quantity > s.Quantity.Maximum)
                {
                    sb.Append($"Quantity {o.Quantity} cannot be above maximum {s.Quantity.Maximum};");
                }

                if ((o.Quantity - s.Quantity.Minimum) % s.Quantity.Increment != 0)
                {
                    sb.Append($"Quantity {o.Quantity} doesn't meet step size {s.Quantity.Increment};");
                }

                if (s.IsIcebergAllowed
                    && o.IcebergQuantity != 0)
                {
                    if (o.IcebergQuantity < s.Quantity.Minimum)
                    {
                        sb.Append($"Iceberg Quantity {o.IcebergQuantity} cannot be below minimum {s.Quantity.Minimum};");
                    }

                    if (o.IcebergQuantity > s.Quantity.Maximum)
                    {
                        sb.Append($"Iceberg Quantity {o.IcebergQuantity} cannot be above maximum {s.Quantity.Maximum};");
                    }

                    if ((o.IcebergQuantity - s.Quantity.Minimum) % s.Quantity.Increment != 0)
                    {
                        sb.Append($"Iceberg Quantity {o.IcebergQuantity} doesn't meet step size {s.Quantity.Increment};");
                    }
                }
            });
        }

        public ClientOrderValidation Build()
        {
            return new ClientOrderValidation(validations);
        }

        public ClientOrderValidationBuilder AddPriceValidation()
        {
            validations.Add((s, o, sb) =>
            {
                if (o.Price < s.Price.Minimum)
                {
                    sb.Append($"Price {o.Price} cannot be below minimum {s.Price.Minimum};");
                }

                if (o.Price > s.Price.Maximum)
                {
                    sb.Append($"Price {o.Price} cannot be above maximum {s.Price.Maximum};");
                }

                if ((o.Price - s.Price.Minimum) % s.Price.Increment != 0)
                {
                    sb.Append($"Price {o.Price} doesn't meet tick size {s.Price.Increment};");
                }
            });

            return this;
        }

        public ClientOrderValidationBuilder AddStopPriceValidation()
        {
            validations.Add((s, o, sb) =>
            {
                if (o.StopPrice < s.Price.Minimum)
                {
                    sb.Append($"Price {o.StopPrice} cannot be below minimum {s.Price.Minimum};");
                }

                if (o.StopPrice > s.Price.Maximum)
                {
                    sb.Append($"Price {o.StopPrice} cannot be above maximum {s.Price.Maximum};");
                }

                if ((o.StopPrice - s.Price.Minimum) % s.Price.Increment != 0)
                {
                    sb.Append($"Price {o.StopPrice} doesn't meet tick size {s.Price.Increment};");
                }
            });

            return this;
        }
    }
}
