using DevelopmentInProgress.TradeView.Interface.Enums;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public interface IOrderBookHelperFactory : IHelperFactory
    {
        IOrderBookHelper GetOrderBookHelper(Exchange exchange);
    }
}
