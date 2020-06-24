using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Service;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Cache
{
    public class SymbolsCacheFactory : ISymbolsCacheFactory
    {
        private IExchangeApiFactory exchangeApiFactory;
        private Dictionary<Exchange, ISymbolsCache> exchangeSymbolsCache;
        private object exchangeSymbolsCacheLock = new object();

        public SymbolsCacheFactory(IExchangeApiFactory exchangeApiFactory)
        {
            this.exchangeApiFactory = exchangeApiFactory;
            exchangeSymbolsCache = new Dictionary<Exchange, ISymbolsCache>();
        }

        public ISymbolsCache GetSymbolsCache(Exchange exchange)
        {
            if (exchangeSymbolsCache.ContainsKey(exchange))
            {
                return exchangeSymbolsCache[exchange];
            }
            else
            {
                lock (exchangeSymbolsCacheLock)
                {
                    if (exchangeSymbolsCache.ContainsKey(exchange))
                    {
                        return exchangeSymbolsCache[exchange];
                    }
                    else
                    {
                        var wpfExchangeService = new WpfExchangeService(new ExchangeService(exchangeApiFactory));
                        var symbolsCache = new SymbolsCache(exchange, wpfExchangeService);
                        exchangeSymbolsCache.Add(exchange, symbolsCache);
                        return symbolsCache;
                    }
                }
            }
        }
    }
}