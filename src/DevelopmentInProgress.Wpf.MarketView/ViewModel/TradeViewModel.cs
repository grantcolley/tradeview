using DevelopmentInProgress.MarketView.Interface.Helpers;
using DevelopmentInProgress.Wpf.MarketView.Events;
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
        private List<Symbol> symbols;
        private Symbol selectedSymbol;
        private Account account;
        private bool disposed;
        private bool isLoading;

        public TradeViewModel(IExchangeService exchangeService)
            : base(exchangeService)
        {
            symbolsCancellationTokenSource = new CancellationTokenSource();

            GetSymbols();
        }

        public event EventHandler<TradeEventArgs> OnTradeNotification;

        public Account Account
        {
            get { return account; }
            set
            {
                if (account != value)
                {
                    account = value;
                }
            }
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
                }
            }
        }

        public string[] OrderTypes
        {
            get { return OrderTypeHelper.OrderTypes(); }
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
                OnException(e);
            }

            IsLoading = false;
        }

        private void OnException(Exception exception)
        {
            var onTradeNotification = OnTradeNotification;
            onTradeNotification?.Invoke(this, new TradeEventArgs { Exception = exception });
        }
    }
}
