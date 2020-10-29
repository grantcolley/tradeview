using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Wpf.Common.Cache;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Test.Helper
{
    public class SymbolsCacheFactoryTest : ISymbolsCacheFactory
    {
        private readonly IWpfExchangeService exchangeService;

        public SymbolsCacheFactoryTest(IWpfExchangeService exchangeService)
        {
            this.exchangeService = exchangeService;
        }

        public ISymbolsCache GetSymbolsCache(Exchange exchange)
        {
            return new SymbolsCache(exchange, exchangeService);
        }

        public Task SubscribeAccountsAssets()
        {
            throw new System.NotImplementedException();
        }
    }
}
