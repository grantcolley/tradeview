using System;
using DevelopmentInProgress.MarketView.Interface.Helpers;
using DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.MarketView.Interface.Validation
{
    public class ValidateLimit : ValidateOrder, IValidateClientOrder
    {
        public new void Validate(Symbol symbol, ClientOrder clientOrder)
        {
            base.Validate(symbol, clientOrder);

            if (clientOrder.Price.Equals(0))
            {
                throw new Exception($"{OrderHelper.GetOrderTypeName(clientOrder.Type)} order not valid: Price cannot be 0");
            }
        }
    }
}
