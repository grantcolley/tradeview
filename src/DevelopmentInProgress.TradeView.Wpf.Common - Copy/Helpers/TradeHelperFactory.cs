using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public class TradeHelperFactory : ITradeHelperFactory
    {
        private readonly Dictionary<Exchange, ITradeHelper> tradeHelpers;

        public TradeHelperFactory(IExchangeApiFactory exchangeApiFactory)
        {
            tradeHelpers = new Dictionary<Exchange, ITradeHelper>();
            tradeHelpers.Add(Exchange.Binance, new TradeHelper());
            tradeHelpers.Add(Exchange.Kucoin, new TradeHelper());
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
