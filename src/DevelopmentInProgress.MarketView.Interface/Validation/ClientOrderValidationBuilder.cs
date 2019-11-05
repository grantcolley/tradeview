using DevelopmentInProgress.MarketView.Interface.Extensions;
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
                    sb.Append("Order has no symbol;");
                }
                else if (!o.Symbol.Equals($"{s.ExchangeSymbol}"))
                {
                    sb.Append($"Order {o.Symbol} validation symbol {s.BaseAsset.Symbol}{s.QuoteAsset.Symbol} mismatch;");
                }

                if (!s.OrderTypes.Contains(o.Type))
                {
                    sb.Append($"{o.Type} order is not permitted;");
                }

                if (o.Side.Equals(OrderSide.Sell)
                    && !o.BaseAccountBalance.HasAvailableQuantity(o.Quantity))
                {
                    sb.Append($"Insufficient quantity to sell: {o.Quantity} is greater than the available quantity {o.BaseAccountBalance.Free};");
                }
                else if (o.Side.Equals(OrderSide.Buy)
                    && !o.QuoteAccountBalance.HasAvailableFunds(o.Price, o.Quantity))
                {
                    switch(o.Type)
                    {
                        case OrderType.Market:
                        case OrderType.TakeProfit:
                        case OrderType.StopLoss:
                            sb.Append($"Insufficient funds to buy: Indicative cost {o.Price * o.Quantity} is greater than the available funds {o.QuoteAccountBalance.Free};");
                            break;
                        case OrderType.Limit:
                        case OrderType.LimitMaker:
                        case OrderType.StopLossLimit:
                        case OrderType.TakeProfitLimit:
                            sb.Append($"Insufficient funds to buy: {o.Price * o.Quantity} is greater than the available funds {o.QuoteAccountBalance.Free};");
                            break;
                    }
                }

                if (o.Quantity < s.Quantity.Minimum)
                {
                    sb.Append($"Quantity {o.Quantity} is below the minimum {s.Quantity.Minimum};");
                }

                if (o.Quantity > s.Quantity.Maximum)
                {
                    sb.Append($"Quantity {o.Quantity} is above the maximum {s.Quantity.Maximum};");
                }

                if ((o.Quantity - s.Quantity.Minimum) % s.Quantity.Increment != 0)
                {
                    sb.Append($"Quantity {o.Quantity} must be in multiples of the step size {s.Quantity.Increment};");
                }

                var notional = o.Price * o.Quantity;
                if (notional < s.NotionalMinimumValue)
                {
                    sb.Append($"Notional {notional} is less than the minimum notional {s.NotionalMinimumValue};");
                }

                // NOTE: Timestamp is mandatory...
            });
        }

        public ClientOrderValidation Build()
        {
            return new ClientOrderValidation(validations);
        }

        //public ClientOrderValidationBuilder AddPriceValidation()
        //{
        //    validations.Add((s, o, sb) =>
        //    {
        //        if (o.Price < s.Price.Minimum)
        //        {
        //            sb.Append($"Price {o.Price} cannot be below the minimum {s.Price.Minimum};");
        //        }

        //        if (o.Price > s.Price.Maximum)
        //        {
        //            sb.Append($"Price {o.Price} cannot be above the maximum {s.Price.Maximum};");
        //        }

        //        if ((o.Price - s.Price.Minimum) % s.Price.Increment != 0)
        //        {
        //            sb.Append($"Price {o.Price} doesn't meet the tick size {s.Price.Increment};");
        //        }
        //    });

        //    return this;
        //}

        public ClientOrderValidationBuilder AddStopPriceValidation()
        {
            validations.Add((s, o, sb) =>
            {
                if (o.Side == OrderSide.Buy
                && !o.QuoteAccountBalance.HasAvailableFunds(o.StopPrice, o.Quantity))
                {
                    sb.Append($"Insufficient funds to buy: {o.StopPrice * o.Quantity} is greater than the available {o.QuoteAccountBalance.Free};");
                }

                //if (o.StopPrice < s.Price.Minimum)
                //{
                //    sb.Append($"Stop Price {o.StopPrice} cannot be below the minimum {s.Price.Minimum};");
                //}

                //if (o.StopPrice > s.Price.Maximum)
                //{
                //    sb.Append($"Stop Price {o.StopPrice} cannot be above the maximum {s.Price.Maximum};");
                //}

                //if ((o.StopPrice - s.Price.Minimum) % s.Price.Increment != 0)
                //{
                //    sb.Append($"Stop Price {o.StopPrice} doesn't meet the tick size {s.Price.Increment};");
                //}
            });

            return this;
        }

        public ClientOrderValidationBuilder AddIcebergValidation()
        {
            throw new NotImplementedException("Iceberg validation not implemented yet.");

            validations.Add((s, o, sb) =>
            {
                if (s.IsIcebergAllowed
                    && o.IcebergQuantity != 0)
                {
                    if(!(o.Type == OrderType.Limit
                        || o.Type == OrderType.LimitMaker))
                    {
                        sb.Append($"Order type must be Limit or Limit Maker to be an iceberg");
                    }

                    if(o.TimeInForce != TimeInForce.GTC)
                    {
                        sb.Append($"Iceberg order time in force must be GTC");
                    }

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

            return this;
        }
    }
}
