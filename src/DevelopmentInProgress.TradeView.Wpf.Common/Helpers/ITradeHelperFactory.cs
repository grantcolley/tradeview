using DevelopmentInProgress.TradeView.Interface.Enums;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public interface ITradeHelperFactory : IHelperFactory
    {
        ITradeHelper GetTradeHelper();
        ITradeHelper GetTradeHelper(Exchange exchange);
    }
}
