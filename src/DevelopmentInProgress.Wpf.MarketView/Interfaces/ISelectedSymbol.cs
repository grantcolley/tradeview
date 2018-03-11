using DevelopmentInProgress.Wpf.MarketView.Model;

namespace DevelopmentInProgress.Wpf.MarketView.Interfaces
{
    public interface ISelectedSymbol : INotify<Symbol>
    {
        new void Notify(Symbol symbol);
    }
}