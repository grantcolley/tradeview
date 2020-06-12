using DevelopmentInProgress.TradeView.Core.Enums;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public interface IOrderBookHelperFactory : IHelperFactory
    {
        IOrderBookHelper GetOrderBookHelper(Exchange exchange);
    }
}
