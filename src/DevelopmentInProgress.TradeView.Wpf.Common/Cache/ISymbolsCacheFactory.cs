using DevelopmentInProgress.TradeView.Interface.Enums;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Cache
{
    public interface ISymbolsCacheFactory
    {
        ISymbolsCache GetSymbolsCache(Exchange exchange);
    }
}
