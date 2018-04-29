using DevelopmentInProgress.Wpf.MarketView.Events;
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
        private List<Symbol> symbols;
        private Symbol selectedSymbol;
        private User user;
        private bool showFavourites;
        private bool isLoadingSymbols;
        private bool disposed;

        public SymbolsViewModel(IExchangeService exchangeService)
            : base(exchangeService)
        {
            symbolsCancellationTokenSource = new CancellationTokenSource();

            GetSymbols();
        }

        public event EventHandler<SymbolsEventArgs> OnSymbolsNotification;

        public List<Symbol> Symbols
        {
            get { return symbols; }
            set
            {
                if (symbols != value)
                {
                    symbols = value;
                    if(symbols != null)
                    {
                        OnLoadedSymbols(symbols);
                    }

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
                    OnSelectedSymbol(selectedSymbol);
                    OnPropertyChanged("SelectedSymbol");
                }
            }
        }

        public User User
        {
            get { return user; }
            set
            {
                if (user != value)
                {
                    user = value;
                    SetPreferences();
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

        public void SetUser(User user)
        {
            try
            {
                User = user;
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        private async void GetSymbols()
        {
            IsLoadingSymbols = true;

            try
            {
                var results = await ExchangeService.GetSymbols24HourStatisticsAsync(symbolsCancellationTokenSource.Token);

                Symbols = new List<Symbol>(results);

                ExchangeService.SubscribeStatistics(symbols, OnException, symbolsCancellationTokenSource.Token);

                SetPreferences();
            }
            catch(Exception ex)
            {
                OnException(ex);
            }

            IsLoadingSymbols = false;
        }

        private void OnException(Exception exception)
        {
            var onSymbolsNotification = OnSymbolsNotification;
            onSymbolsNotification?.Invoke(this, new SymbolsEventArgs { Exception = exception });
        }

        private void OnSelectedSymbol(Symbol symbol)
        {
            var onSymbolsNotification = OnSymbolsNotification;
            onSymbolsNotification?.Invoke(this, new SymbolsEventArgs { Value = symbol });
        }

        private void OnLoadedSymbols(List<Symbol> symbols)
        {
            var onSymbolsNotification = OnSymbolsNotification;
            onSymbolsNotification?.Invoke(this, new SymbolsEventArgs { Symbols = symbols });
        }

        private void SetPreferences()
        {
            if (user != null 
                && user.Preferences != null 
                && Symbols != null 
                && Symbols.Any())
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

                    if (!string.IsNullOrWhiteSpace(user.Preferences.SelectedSymbol))
                    {
                        var symbol = Symbols.FirstOrDefault(s => s.Name.Equals(user.Preferences.SelectedSymbol));
                        if (symbol != null)
                        {
                            SelectedSymbol = symbol;
                        }
                    }
                }
            }
        }
    }
}
