using DevelopmentInProgress.TradeView.Interface.Enums;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public interface ITradeHelperFactory
    {
        ITradeHelper GetTradeHelper(Exchange exchange);
    }
}
