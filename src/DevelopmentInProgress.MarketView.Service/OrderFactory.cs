using Binance;

namespace DevelopmentInProgress.MarketView.Service
{
    public static class OrderFactory
    {
        public static ClientOrder GetOrder(Interface.Model.User user, Interface.Model.ClientOrder clientOrder)
        {
            var apiUser = new BinanceApiUser(user.ApiKey, user.ApiSecret);
            return new LimitOrder(apiUser);
        }
    }
}
