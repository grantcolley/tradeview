using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class SymbolsViewModel : BaseViewModel
    {
        private List<Symbol> symbols;
        private Symbol selectedSymbol;
        private UserAccount accountPreferences;
        private bool showFavourites;
        private bool isLoadingSymbols;
        private bool disposed;

        public SymbolsViewModel(IWpfExchangeService exchangeService)
            : base(exchangeService)
        {
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

        public UserAccount AccountPreferences
        {
            get { return accountPreferences; }
            set
            {
                if (accountPreferences != value)
                {
                    accountPreferences = value;
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
                ExchangeService.OnSubscribeSymbolsException -= SubscribeStatisticsException;
            }

            disposed = true;
        }

        public void SetAccount(UserAccount userAccount)
        {
            try
            {
                AccountPreferences = userAccount;
            }
            catch (Exception ex)
            {
                OnException("SymbolsViewModel.SetUser", ex);
            }
        }

        private async void GetSymbols()
        {
            IsLoadingSymbols = true;

            try
            {
                ExchangeService.OnSubscribeSymbolsException += SubscribeStatisticsException;

                var results = await ExchangeService.GetSymbolsSubscription();

                Symbols = new List<Symbol>(results);
                
                SetPreferences();
            }
            catch(Exception ex)
            {
                OnException("SymbolsViewModel.GetSymbols", ex);
            }

            IsLoadingSymbols = false;
        }

        private void SubscribeStatisticsException(object sender, Exception exception)
        {
            OnException("SymbolsViewModel.GetSymbols - ExchangeService.GetSymbolsSubscription", exception);
        }

        private void OnException(string message, Exception exception)
        {
            var onSymbolsNotification = OnSymbolsNotification;
            onSymbolsNotification?.Invoke(this, new SymbolsEventArgs { Message = message, Exception = exception });
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
            if (accountPreferences != null 
                && accountPreferences.Preferences != null 
                && Symbols != null 
                && Symbols.Any())
            {
                if (accountPreferences.Preferences.FavouriteSymbols != null
                    && accountPreferences.Preferences.FavouriteSymbols.Any())
                {
                    Func<Symbol, string, Symbol> f = ((s, p) =>
                    {
                        s.IsFavourite = true;
                        return s;
                    });

                    (from s in Symbols join fs in accountPreferences.Preferences.FavouriteSymbols on s.Name equals fs.ToString() select f(s, fs)).ToList();

                    ShowFavourites = accountPreferences.Preferences.ShowFavourites;

                    if (!string.IsNullOrWhiteSpace(accountPreferences.Preferences.SelectedSymbol))
                    {
                        var symbol = Symbols.FirstOrDefault(s => s.Name.Equals(accountPreferences.Preferences.SelectedSymbol));
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
