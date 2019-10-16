using DevelopmentInProgress.MarketView.Interface.Enums;

namespace DevelopmentInProgress.Wpf.Common.Helpers
{
    public interface IOrderBookHelperFactory
    {
        IOrderBookHelper GetOrderBookHelper(Exchange exchange);
    }
}
