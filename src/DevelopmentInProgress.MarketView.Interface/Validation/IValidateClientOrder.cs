using DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.MarketView.Interface.Validation
{
    public interface IValidateClientOrder
    {
        bool TryValidate(Symbol symbol, ClientOrder clientOrder, out string message);
    }
}