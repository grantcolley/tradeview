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
                        wpfExchangeService.SubscribeStatistics(symbols, SubscribeStatisticsException, subscribeSymbolsCxlTokenSrc.Token);
                    }
                }
            }

            return symbols;
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
