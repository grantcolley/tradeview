using DevelopmentInProgress.Wpf.Common.Cache;

namespace DevelopmentInProgress.Wpf.MarketView.Test
{
    public static class SymbolsCacheHelper
    {
        public static ISymbolsCache GetSymbolsCache()
        {
            return new SymbolsCache();
        }
    }
}
