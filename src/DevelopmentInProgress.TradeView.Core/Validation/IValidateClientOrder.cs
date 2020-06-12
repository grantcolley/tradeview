using DevelopmentInProgress.TradeView.Core.Model;

namespace DevelopmentInProgress.TradeView.Core.Validation
{
    public interface IValidateClientOrder
    {
        bool TryValidate(Symbol symbol, ClientOrder clientOrder, out string message);
    }
}