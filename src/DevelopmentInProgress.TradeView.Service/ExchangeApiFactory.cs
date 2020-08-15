using DevelopmentInProgress.TradeView.Api.Binance;
using DevelopmentInProgress.TradeView.Api.Kucoin;
using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Service
{
    public class ExchangeApiFactory : IExchangeApiFactory
    {
        public IExchangeApi GetExchangeApi(Exchange exchange)
        {
            return exchange switch
            {
                Exchange.Binance => new BinanceExchangeApi(),
                Exchange.Kucoin => new KucoinExchangeApi(),
                _ => throw new NotImplementedException(),
            };
        }

        public Dictionary<Exchange, IExchangeApi> GetExchanges()
        {
            var exchanges = new Dictionary<Exchange, IExchangeApi>
            {
                { Exchange.Binance, GetExchangeApi(Exchange.Binance) },
                { Exchange.Kucoin, GetExchangeApi(Exchange.Kucoin) }
            };

            return exchanges;
        }
    }
}
