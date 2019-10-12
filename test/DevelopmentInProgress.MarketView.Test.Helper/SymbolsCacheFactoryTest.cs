using DevelopmentInProgress.MarketView.Interface.Enums;
using DevelopmentInProgress.Wpf.Common.Cache;
using DevelopmentInProgress.Wpf.Common.Services;

namespace DevelopmentInProgress.MarketView.Test.Helper
{
    public class SymbolsCacheFactoryTest : ISymbolsCacheFactory
    {
        private IWpfExchangeService exchangeService;

        public SymbolsCacheFactoryTest(IWpfExchangeService exchangeService)
        {
            this.exchangeService = exchangeService;
        }

        public ISymbolsCache GetSymbolsCache(Exchange exchange)
        {
            return new SymbolsCache(exchange, exchangeService);
        }
    }
}
