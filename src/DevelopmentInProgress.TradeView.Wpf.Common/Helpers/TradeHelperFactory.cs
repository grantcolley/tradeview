using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public class TradeHelperFactory : ITradeHelperFactory
    {
        private readonly Dictionary<Exchange, ITradeHelper> tradeHelpers;

        public HelperFactoryType HelperFactoryType { get { return HelperFactoryType.TradeHelper; } }

        public TradeHelperFactory(IExchangeApiFactory exchangeApiFactory)
        {
            if (exchangeApiFactory == null)
            {
                throw new ArgumentNullException(nameof(exchangeApiFactory));
            }

            tradeHelpers = new Dictionary<Exchange, ITradeHelper>()
            {
                { Exchange.Binance, new TradeHelper() },
                { Exchange.Kucoin, new TradeHelper() }
            };
        }

        public ITradeHelper GetTradeHelper(Exchange exchange)
        {
            return tradeHelpers[exchange];
        }

        public ITradeHelper GetTradeHelper()
        {
            return new TradeHelper();
        }
    }
}
