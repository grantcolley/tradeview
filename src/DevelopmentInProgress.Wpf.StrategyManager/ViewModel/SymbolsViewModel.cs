using DevelopmentInProgress.Wpf.Common.Cache;
using DevelopmentInProgress.Wpf.Common.ViewModel;
using DevelopmentInProgress.Wpf.StrategyManager.Events;
using DevelopmentInProgress.Wpf.StrategyManager.Model;
using System;

namespace DevelopmentInProgress.Wpf.StrategyManager.ViewModel
{
    public class SymbolsViewModel : BaseViewModel
    {
        private Strategy strategy;
        private ISymbolsCache symbolsCache;
        private bool isLoadingSymbols;
        private bool disposed;

        public SymbolsViewModel(Strategy strategy, ISymbolsCache symbolsCache)
        {
            this.strategy = strategy;

            GetSymbols();
        }

        public event EventHandler<StrategyEventArgs> OnSymbolsNotification;

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

        private async void GetSymbols()
        {
            IsLoadingSymbols = true;

            try
            {
                symbolsCache.OnSymbolsCacheException += SymbolsCacheException;

                var results = await symbolsCache.GetSymbols();

                //Symbols = new List<Symbol>(results);
            }
            catch (Exception ex)
            {
                OnException("SymbolsViewModel.GetSymbols", ex);
            }

            IsLoadingSymbols = false;
        }

        private void SymbolsCacheException(object sender, Exception exception)
        {
            OnException("SymbolsViewModel.GetSymbols - SymbolsCache.GetSymbols", exception);
        }

        private void OnException(string message, Exception exception)
        {
            var onSymbolsNotification = OnSymbolsNotification;
            onSymbolsNotification?.Invoke(this, new StrategyEventArgs { Message = message, Exception = exception });
        }
    }
}
