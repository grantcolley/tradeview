using DevelopmentInProgress.MarketView.Interface.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Validation
{
    public interface IValidateClientOrder
    {
        bool TryValidate(Symbol symbol, ClientOrder clientOrder, out string message);
    }
}