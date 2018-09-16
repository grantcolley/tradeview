using DevelopmentInProgress.Wpf.Common.Cache;
using DevelopmentInProgress.Wpf.Common.Services;

namespace DevelopmentInProgress.Wpf.MarketView.Test
{
    public static class SymbolsCacheHelper
    {
        public static ISymbolsCache GetSymbolsCache(IWpfExchangeService wpfExchangeService)
        {
            return new SymbolsCache(wpfExchangeService);
        }
    }
}
