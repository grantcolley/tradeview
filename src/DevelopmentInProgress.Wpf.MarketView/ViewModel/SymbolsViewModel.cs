using DevelopmentInProgress.Wpf.MarketView.Interfaces;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Personalise;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class SymbolsViewModel : BaseViewModel
    {
        private CancellationTokenSource symbolsCancellationTokenSource;
        private Action<Exception> exception;
        private List<Symbol> symbols;
        private Symbol selectedSymbol;
        private ISelectedSymbol selectedSymbolNotification;
        private User user;
        private bool showFavourites;
        private bool isLoadingSymbols;
        private bool disposed;

        public SymbolsViewModel(User user, IExchangeService exchangeService, ISelectedSymbol selectedSymbolNotification, Action<Exception> exception)
            : base(exchangeService)
        {
            this.user = user;
            this.selectedSymbolNotification = selectedSymbolNotification;
            this.exception = exception;

            symbolsCancellationTokenSource = new CancellationTokenSource();

            GetSymbols();
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
                    selectedSymbolNotification.Notify(selectedSymbol);
                }
            }
        }

        public bool ShowFavourites
        {
            get { return showFavourites; }
            set
            {
                if (showFavourites != value)
                {
                    showFavourites = value;
                    if (showFavourites)
                    {
                        Symbols.ForEach(s => s.IsVisible = s.IsFavourite);
                    }
                    else
                    {
                        Symbols.ForEach(s => s.IsVisible = true);
                    }

                    OnPropertyChanged("ShowFavourites");
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
        
        private async void GetSymbols()
        {
            IsLoadingSymbols = true;

            try
            {
                var results = await ExchangeService.GetSymbols24HourStatisticsAsync(symbolsCancellationTokenSource.Token);

                Symbols = new List<Symbol>(results);

                ExchangeService.SubscribeStatistics(symbols, exception, symbolsCancellationTokenSource.Token);

                if (user != null
                    && user.Preferences != null)
                {
                    if (user.Preferences.FavouriteSymbols != null
                        && user.Preferences.FavouriteSymbols.Any())
                    {
                        Func<Symbol, string, Symbol> f = ((s, p) =>
                        {
                            s.IsFavourite = true;
                            return s;
                        });

                        (from s in Symbols join fs in user.Preferences.FavouriteSymbols on s.Name equals fs.ToString() select f(s, fs)).ToList();

                        ShowFavourites = user.Preferences.ShowFavourites;
                    }
                }
            }
            catch(Exception e)
            {
                exception.Invoke(e);
            }

            IsLoadingSymbols = false;
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
