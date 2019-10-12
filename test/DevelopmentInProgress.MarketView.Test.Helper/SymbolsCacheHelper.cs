using DevelopmentInProgress.MarketView.Interface.Enums;
using DevelopmentInProgress.Wpf.Common.Cache;
using DevelopmentInProgress.Wpf.Common.Services;

namespace DevelopmentInProgress.MarketView.Test.Helper
{
    public static class SymbolsCacheHelper
    {
        public static ISymbolsCache GetSymbolsCache(IWpfExchangeService wpfExchangeService)
        {
            return new SymbolsCache(Exchange.Test, wpfExchangeService);
        }
    }
}
