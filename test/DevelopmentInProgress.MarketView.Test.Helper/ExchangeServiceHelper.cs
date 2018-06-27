using DevelopmentInProgress.MarketView.Interface.Interfaces;

namespace DevelopmentInProgress.MarketView.Test.Helper
{
    public static class ExchangeServiceHelper
    {
        public static IExchangeService GetExchangeService()
        {
            return GetExchangeService(ExchangeServiceType.Standard);
        }

        public static IExchangeService GetExchangeService(ExchangeServiceType exchangeServiceType)
        {
            switch(exchangeServiceType)
            {
                case ExchangeServiceType.Standard:
                    return new ExchangeService();
                case ExchangeServiceType.SubscribeAccountInfo:
                    return new ExchangeServiceSubscribeAccountInfoAccount();
                case ExchangeServiceType.SymbolsViewModel:
                    return new ExchangeServiceSymbolsViewModel();
                case ExchangeServiceType.UpdateOrders:
                    return new ExchangeServiceUpdateOrders();
                case ExchangeServiceType.PlaceOrderException:
                    return new ExchangeServicePlaceOrderException();
                case ExchangeServiceType.SubscribeOrderBookAggregateTrades:
                    return new ExchangeServiceSubscribeOrderBookAggregateTrades();
                default:
                    return new ExchangeService();
            }
        }
    }
}
