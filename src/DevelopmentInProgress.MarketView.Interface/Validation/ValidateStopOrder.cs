using System;
using DevelopmentInProgress.MarketView.Interface.Helpers;
using DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.MarketView.Interface.Validation
{
    public class ValidateStopOrder : ValidateOrder, IValidateClientOrder
    {
        public new void Validate(ClientOrder clientOrder)
        {
            base.Validate(clientOrder);

            if (clientOrder.StopPrice.Equals(0))
            {
                throw new Exception($"{OrderHelper.GetOrderTypeName(clientOrder.Type)} order not valid: Stop price cannot be 0");
            }
        }
    }
}
