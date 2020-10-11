using DevelopmentInProgress.TradeView.Core.Enums;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Cache
{
    public interface ISymbolsCacheFactory
    {
        ISymbolsCache GetSymbolsCache(Exchange exchange);
        Task SubscribeAccountsAssets();
    }
}
