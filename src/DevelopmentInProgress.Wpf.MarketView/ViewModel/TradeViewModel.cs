using DevelopmentInProgress.MarketView.Interface.Helpers;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class TradeViewModel : BaseViewModel
    {
        private CancellationTokenSource symbolsCancellationTokenSource;
        private Action<Exception> exception;
        private List<Symbol> symbols;
        private Symbol selectedSymbol;
        private bool disposed;
        private bool isLoading;

        public TradeViewModel(Account account, IExchangeService exchangeService, Action<Exception> exception)
            : base(exchangeService)
        {
            this.exception = exception;

            symbolsCancellationTokenSource = new CancellationTokenSource();

            GetSymbols();
        }

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged("IsLoading");
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

        public Symbol SelectedSymbol
        {
            get { return selectedSymbol; }
            set
            {
                if (selectedSymbol != value)
                {
                    selectedSymbol = value;
                    //selectedSymbolNotification.Notify(selectedSymbol);
                }
            }
        }

        public string[] OrderTypes
        {
            get { return OrderTypeHelper.OrderTypes(); }
        }

        private async void GetSymbols()
        {
            IsLoading = true;

            try
            {
                var results = await ExchangeService.GetSymbolsAsync(symbolsCancellationTokenSource.Token);

                Symbols = new List<Symbol>(results);
            }
            catch (Exception e)
            {
                exception.Invoke(e);
            }

            IsLoading = false;
        }

        public override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (symbolsCancellationTokenSource != null
                    && !symbolsCancellationTokenSource.IsCancellationRequested)
                {
                    symbolsCancellationTokenSource.Cancel();
                }
            }

            disposed = true;
        }
    }
}
