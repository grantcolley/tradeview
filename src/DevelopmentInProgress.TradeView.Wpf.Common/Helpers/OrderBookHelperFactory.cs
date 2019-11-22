using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public class OrderBookHelperFactory : IOrderBookHelperFactory
    {
        private Dictionary<Exchange, IOrderBookHelper> orderBookHelpers;

        public OrderBookHelperFactory(IExchangeApiFactory exchangeApiFactory)
        {
            orderBookHelpers = new Dictionary<Exchange, IOrderBookHelper>();
            orderBookHelpers.Add(Exchange.Binance, new BinanceOrderBookHelper());
            orderBookHelpers.Add(Exchange.Kucoin, new KucoinOrderBookHelper(exchangeApiFactory.GetExchangeApi(Exchange.Kucoin)));
        }

        public IOrderBookHelper GetOrderBookHelper(Exchange exchange)
        {
            return orderBookHelpers[exchange];
        }
    }
}
