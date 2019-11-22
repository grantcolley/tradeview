using DevelopmentInProgress.TradeView.Wpf.Common.Cache;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;

namespace DevelopmentInProgress.TradeView.Test.Helper
{
    public static class SymbolsCacheFactoryHelper
    {
        public static ISymbolsCacheFactory GetSymbolsCachefactory(IWpfExchangeService exchangeService)
        {
            return new SymbolsCacheFactoryTest(exchangeService);
        }
    }
}
