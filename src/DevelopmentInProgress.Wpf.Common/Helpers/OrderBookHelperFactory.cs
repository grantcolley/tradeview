using DevelopmentInProgress.MarketView.Interface.Enums;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.Common.Helpers
{
    public class OrderBookHelperFactory : IOrderBookHelperFactory
    {
        private Dictionary<Exchange, IOrderBookHelper> orderBookHelpers;

        public OrderBookHelperFactory()
        {
            orderBookHelpers = new Dictionary<Exchange, IOrderBookHelper>();
            orderBookHelpers.Add(Exchange.Binance, new BinanceOrderBookHelper());
        }

        public IOrderBookHelper GetOrderBookHelper(Exchange exchange)
        {
            return orderBookHelpers[exchange];
        }
    }
}
