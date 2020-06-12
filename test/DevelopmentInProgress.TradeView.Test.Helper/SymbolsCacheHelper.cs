using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Wpf.Common.Cache;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;

namespace DevelopmentInProgress.TradeView.Test.Helper
{
    public static class SymbolsCacheHelper
    {
        public static ISymbolsCache GetSymbolsCache(IWpfExchangeService wpfExchangeService)
        {
            return new SymbolsCache(Exchange.Test, wpfExchangeService);
        }
    }
}
