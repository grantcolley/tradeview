using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public class TradeHelperFactory : ITradeHelperFactory
    {
        private readonly Dictionary<Exchange, ITradeHelper> tradeHelpers;

        public TradeHelperFactory(IExchangeApiFactory exchangeApiFactory)
        {
            tradeHelpers = new Dictionary<Exchange, ITradeHelper>();
            tradeHelpers.Add(Exchange.Binance, new BinanceTradeHelper());
            tradeHelpers.Add(Exchange.Kucoin, new KucoinTradeHelper(exchangeApiFactory.GetExchangeApi(Exchange.Kucoin)));
        }

        public ITradeHelper GetTradeHelper(Exchange exchange)
        {
            return tradeHelpers[exchange];
        }
    }
}
