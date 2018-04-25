using DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.MarketView.Interface.Validation
{
    public class ValidateMarket : ValidateOrder, IValidateClientOrder
    {
        public new void Validate(ClientOrder clientOrder)
        {
            base.Validate(clientOrder);
        }
    }
}
