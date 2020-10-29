using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Service;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Cache
{
    public class SymbolsCacheFactory : ISymbolsCacheFactory
    {
        private readonly IExchangeApiFactory exchangeApiFactory;
        private readonly Dictionary<Exchange, ISymbolsCache> exchangeSymbolsCache;
        private readonly IAccountsService accountsService;
        private readonly object exchangeSymbolsCacheLock = new object();

        public SymbolsCacheFactory(IExchangeApiFactory exchangeApiFactory, IAccountsService accountsService)
        {
            this.exchangeApiFactory = exchangeApiFactory;
            this.accountsService = accountsService;
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

        public async Task SubscribeAccountsAssets()
        {
            var userAccounts = await accountsService.GetAccountsAsync().ConfigureAwait(false);

            var exchangeAccounts = userAccounts.Accounts.GroupBy(a => a.Exchange, a => a);

            foreach(var exchangeAccount in exchangeAccounts)
            {
                var symbolCache = GetSymbolsCache(exchangeAccount.Key);
                symbolCache.SubscribeAccountsAssets(exchangeAccount.ToList());
            }
        }
    }
}