using DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.MarketView.Interface.Validation
{
    public interface IValidateClientOrder
    {
        void Validate(Symbol symbol, ClientOrder clientOrder);
    }
}
