using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Common.Services;

namespace DevelopmentInProgress.Wpf.Common.Cache
{
    public class SymbolsCache : ISymbolsCache, IDisposable
    {
        private IWpfExchangeService wpfExchangeService;
        private List<Symbol> symbols;
        private Symbol btcUsdt;
        private CancellationTokenSource subscribeSymbolsCxlTokenSrc = new CancellationTokenSource();
        private object lockSubscriptions = new object();
        private bool disposed;

        public SymbolsCache(IWpfExchangeService wpfExchangeService)
        {
            this.wpfExchangeService = wpfExchangeService;

            symbols = new List<Symbol>();
        }

        public event EventHandler<Exception> OnSymbolsCacheException;

        public async Task<List<Symbol>> GetSymbols()
        {
            if (!symbols.Any())
            {
                var results = await wpfExchangeService.GetSymbols24HourStatisticsAsync(subscribeSymbolsCxlTokenSrc.Token);

                lock (lockSubscriptions)
                {
                    if (!symbols.Any())
                    {
                        symbols.AddRange(results);

                        btcUsdt = symbols.Single(s => s.Name.Equals("BTCUSDT"));

                        wpfExchangeService.SubscribeStatistics(symbols, SubscribeStatisticsException, subscribeSymbolsCxlTokenSrc.Token);
                    }
                }
            }

            return symbols;
        }

        public decimal USDTValueBalances(List<AccountBalance> balances)
        {
            if(btcUsdt == null
                || !symbols.Any())
            {
                return 0m;
            }

            decimal value = 0m;
            decimal btc = 0m;

            foreach(var balance in balances)
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
                    if(symbol != null)
                    {
                        btc += symbol.SymbolStatistics.BidPrice * qty;
                    }
                }
            }

            value = btcUsdt.SymbolStatistics.BidPrice * btc;

            return Math.Round(value, 2);
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
