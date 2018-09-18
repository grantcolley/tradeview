using DevelopmentInProgress.Wpf.Common.Cache;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Common.ViewModel;
using DevelopmentInProgress.Wpf.StrategyManager.Events;
using DevelopmentInProgress.Wpf.StrategyManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.StrategyManager.ViewModel
{
    public class SymbolsViewModel : BaseViewModel
    {
        private Strategy strategy;
        private ISymbolsCache symbolsCache;
        private List<Symbol> symbols;
        private bool isLoadingSymbols;
        private bool disposed;

        public SymbolsViewModel(ISymbolsCache symbolsCache)
        {
            this.symbolsCache = symbolsCache;
        }

        public event EventHandler<StrategyEventArgs> OnSymbolsNotification;

        public Strategy Strategy
        {
            get { return strategy; }
            set
            {
                if (strategy != value)
                {
                    strategy = value;
                }
            }
        }

        public List<Symbol> Symbols
        {
            get { return symbols; }
            set
            {
                if (symbols != value)
                {
                    symbols = value;
                    OnPropertyChanged("Symbols");
                }
            }
        }

        public bool IsLoadingSymbols
        {
            get { return isLoadingSymbols; }
            set
            {
                if (isLoadingSymbols != value)
                {
                    isLoadingSymbols = value;
                    OnPropertyChanged("IsLoadingSymbols");
                }
            }
        }

        public override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                symbolsCache.OnSymbolsCacheException -= SymbolsCacheException;
            }

            disposed = true;
        }

        public async Task GetSymbols(Strategy arg)
        {
            IsLoadingSymbols = true;

            try
            {
                Strategy = arg;

                symbolsCache.OnSymbolsCacheException += SymbolsCacheException;

                var results = await symbolsCache.GetSymbols();

                var strategySymbols = Strategy.StrategySubscriptions.Select(s => s.Symbol);

                Symbols = new List<Symbol>(results.Where(r => strategySymbols.Contains($"{r.BaseAsset.Symbol}{r.QuoteAsset.Symbol}")));
            }
            catch (Exception ex)
            {
                OnException($"SymbolsViewModel.GetSymbols {ex.Message}", ex);
            }

            IsLoadingSymbols = false;
        }

        private void SymbolsCacheException(object sender, Exception exception)
        {
            OnException($"SymbolsCache.GetSymbols {exception.Message}", exception);
        }

        private void OnException(string message, Exception exception)
        {
            var onSymbolsNotification = OnSymbolsNotification;
            onSymbolsNotification?.Invoke(this, new StrategyEventArgs { Message = message, Exception = exception });
        }
    }
}
