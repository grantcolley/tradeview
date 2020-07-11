using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public class OrderBookHelperFactory : IOrderBookHelperFactory
    {
        private readonly Dictionary<Exchange, IOrderBookHelper> orderBookHelpers;

        public OrderBookHelperFactory(IExchangeApiFactory exchangeApiFactory)
        {
            if(exchangeApiFactory == null)
            {
                throw new ArgumentNullException(nameof(exchangeApiFactory));
            }

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
