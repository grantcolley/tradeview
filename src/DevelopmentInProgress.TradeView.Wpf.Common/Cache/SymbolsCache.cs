using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Interface.Extensions;
using DevelopmentInProgress.TradeView.Interface.Enums;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Cache
{
    public class SymbolsCache : ISymbolsCache, IDisposable
    {
        private const string BTCUSDT = "BTCUSDT";

        private Exchange exchange;
        private IWpfExchangeService wpfExchangeService;
        private CancellationTokenSource subscribeSymbolsCxlTokenSrc = new CancellationTokenSource();
        private List<Symbol> symbols;
        private List<Symbol> subscribedSymbols;
        private Symbol btcUsdt;
        private object lockSubscriptions = new object();
        private bool disposed;

        public SymbolsCache(Exchange exchange, IWpfExchangeService wpfExchangeService)
        {
            this.exchange = exchange;
            this.wpfExchangeService = wpfExchangeService;

            symbols = new List<Symbol>();
            subscribedSymbols = new List<Symbol>();
        }

        public event EventHandler<Exception> OnSymbolsCacheException;

        public async Task<List<Symbol>> GetSymbols(IEnumerable<string> subscriptions)
        {
            // If the cached full symbol list is empty go get them.
            if (!symbols.Any())
            {
                var allSymbols = await wpfExchangeService.GetSymbols24HourStatisticsAsync(exchange, subscribeSymbolsCxlTokenSrc.Token);

                // Can't lock an await so lock after the await 
                // and check the symbols list is still empty
                // before populating with the results.
                lock (lockSubscriptions)
                {
                    if (!symbols.Any())
                    {
                        symbols.AddRange(allSymbols);
                        btcUsdt = symbols.Single(s => s.Name.Equals(BTCUSDT));
                    }
                }
            }

            var newSubs1 = subscriptions.Where(s => !subscribedSymbols.Any(sub => sub.Name.Equals(s))).ToList();

            // Only subscribe to the symbols that aren't already in the subscribed symbols cache.
            if (newSubs1.Any())
            {
                lock(lockSubscriptions)
                {
                    var newSubs2 = subscriptions.Where(s => !subscribedSymbols.Any(sub => sub.Name.Equals(s))).ToList();

                    if(newSubs2.Any())
                    {
                        var newSubSymbols = (from s in symbols join subs in newSubs2 on s.Name equals subs select s).ToList();

                        // If we haven't already subscribed to BTCUSDT then do so now.
                        if(!subscribedSymbols.Contains(btcUsdt)
                            && !newSubs2.Contains(BTCUSDT))
                        {
                            newSubSymbols.Add(btcUsdt);
                        }

                        wpfExchangeService.SubscribeStatistics(exchange, newSubSymbols, SubscribeStatisticsException, subscribeSymbolsCxlTokenSrc.Token);

                        // Add new subscriptions to the cache
                        subscribedSymbols.AddRange(newSubSymbols);
                    }
                }
            }

            // Get the subscriptions from the subscribed symbols cache
            var results = (from s in subscribedSymbols join subs in subscriptions on s.Name equals subs select s).ToList();

            return results;
        }

        public void ValueAccount(Account account)
        {
            if (btcUsdt == null
                || !symbols.Any())
            {
                return;
            }

            decimal usdt = 0m;
            decimal btc = 0m;

            foreach (var balance in account.Balances)
            {
                var qty = balance.Free + balance.Locked;

                if (qty <= 0)
                {
                    continue;
                }

                if (balance.Asset.Equals("BTC"))
                {
                    btc += qty;
                }
                else
                {
                    var symbol = symbols.FirstOrDefault(s => s.Name.Equals($"{balance.Asset}BTC"));
                    if (symbol != null)
                    {
                        btc += symbol.SymbolStatistics.LastPrice * qty;
                    }
                }
            }

            usdt = btcUsdt.SymbolStatistics.LastPrice * btc;

            account.BTCValue = Math.Round(btc, 8);
            account.USDTValue = usdt.Trim(btcUsdt.PricePrecision);
        }

        private void SubscribeStatisticsException(Exception exception)
        {
            var onSubscribeSymbolsException = OnSymbolsCacheException;
            onSubscribeSymbolsException?.Invoke(this, exception);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (!subscribeSymbolsCxlTokenSrc.IsCancellationRequested)
                {
                    subscribeSymbolsCxlTokenSrc.Cancel();
                }

                disposed = false;
            }
        }

    }
}
