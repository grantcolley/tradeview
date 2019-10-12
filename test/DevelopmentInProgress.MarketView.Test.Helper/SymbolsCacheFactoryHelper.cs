using DevelopmentInProgress.Wpf.Common.Cache;
using DevelopmentInProgress.Wpf.Common.Services;

namespace DevelopmentInProgress.MarketView.Test.Helper
{
    public static class SymbolsCacheFactoryHelper
    {
        public static ISymbolsCacheFactory GetSymbolsCachefactory(IWpfExchangeService exchangeService)
        {
            return new SymbolsCacheFactoryTest(exchangeService);
        }
    }
}
