using DevelopmentInProgress.MarketView.Interface.Interfaces;

namespace DevelopmentInProgress.MarketView.Test.Helper
{
    public static class TestExchangeHelper
    {
        public static IExchangeApi GetExchangeApi()
        {
            return GetExchangeApi(ExchangeApiType.Standard);
        }

        public static IExchangeApi GetExchangeApi(ExchangeApiType exchangeApiType)
        {
            switch(exchangeApiType)
            {
                case ExchangeApiType.Standard:
                    return new TestExchangeApi();
                case ExchangeApiType.AccountUpdate:
                    return new TestExchangeApiAccountUpdate();
                default:
                    return new TestExchangeApi();
            }
        }
    }
}
