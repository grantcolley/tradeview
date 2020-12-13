using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestExchangeApiFactory : IExchangeApiFactory
    {
        public IExchangeApi GetExchangeApi(Exchange exchange)
        {
            return exchange switch
            {
                Exchange.Binance => new TestBinanceExchangeApi(),
                Exchange.Test => new TestExchangeApi(),
                _ => throw new NotImplementedException(),
            };
        }

        public Dictionary<Exchange, IExchangeApi> GetExchanges()
        {
            var exchanges = new Dictionary<Exchange, IExchangeApi>
            {
                { Exchange.Binance, GetExchangeApi(Exchange.Binance) },
                { Exchange.Test, GetExchangeApi(Exchange.Test) }
            };

            return exchanges;
        }
    }
}
