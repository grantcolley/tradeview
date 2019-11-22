using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using Prism.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.Utility
{
    public interface ISymbolsLoader
    {
        void ShowSymbols(UserAccount userAccount);
    }
}
