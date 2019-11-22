using DevelopmentInProgress.TradeView.Api.Binance;
using DevelopmentInProgress.TradeView.Api.Kucoin;
using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Service
{
    public class ExchangeApiFactory : IExchangeApiFactory
    {
        public IExchangeApi GetExchangeApi(Exchange exchange)
        {
            switch(exchange)
            {
                case Exchange.Binance:
                    return new BinanceExchangeApi();
                case Exchange.Kucoin:
                    return new KucoinExchangeApi();
                default:
                    throw new NotImplementedException();
            }
        }

        public Dictionary<Exchange, IExchangeApi> GetExchanges()
        {
            var exchanges = new Dictionary<Exchange, IExchangeApi>();
            exchanges.Add(Exchange.Binance, GetExchangeApi(Exchange.Binance));
            exchanges.Add(Exchange.Kucoin, GetExchangeApi(Exchange.Kucoin));
            return exchanges;
        }
    }
}
