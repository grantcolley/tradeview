using DevelopmentInProgress.TradeView.Interface.Model;

namespace DevelopmentInProgress.TradeView.Interface.Validation
{
    public interface IValidateClientOrder
    {
        bool TryValidate(Symbol symbol, ClientOrder clientOrder, out string message);
    }
}