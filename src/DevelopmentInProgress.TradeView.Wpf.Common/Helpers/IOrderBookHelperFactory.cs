using DevelopmentInProgress.TradeView.Interface.Enums;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public interface IOrderBookHelperFactory
    {
        IOrderBookHelper GetOrderBookHelper(Exchange exchange);
    }
}
