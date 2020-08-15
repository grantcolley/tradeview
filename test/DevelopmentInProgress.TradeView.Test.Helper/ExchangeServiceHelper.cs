using DevelopmentInProgress.TradeView.Core.Interfaces;

namespace DevelopmentInProgress.TradeView.Test.Helper
{
    public static class ExchangeServiceHelper
    {
        public static IExchangeService GetExchangeService()
        {
            return GetExchangeService(ExchangeServiceType.Standard);
        }

        public static IExchangeService GetExchangeService(ExchangeServiceType exchangeServiceType)
        {
            return exchangeServiceType switch
            {
                ExchangeServiceType.Standard => new ExchangeService(),
                ExchangeServiceType.SubscribeAccountInfo => new ExchangeServiceSubscribeAccountInfoAccount(),
                ExchangeServiceType.SymbolsViewModel => new ExchangeServiceSymbolsViewModel(),
                ExchangeServiceType.UpdateOrders => new ExchangeServiceUpdateOrders(),
                ExchangeServiceType.PlaceOrderException => new ExchangeServicePlaceOrderException(),
                ExchangeServiceType.SubscribeOrderBookAggregateTrades => new ExchangeServiceSubscribeOrderBookAggregateTrades(),
                _ => new ExchangeService(),
            };
        }
    }
}
