using DevelopmentInProgress.MarketView.Interface.Enums;

namespace DevelopmentInProgress.Wpf.Common.Cache
{
    public interface ISymbolsCacheFactory
    {
        ISymbolsCache GetSymbolsCache(Exchange exchange);
    }
}
