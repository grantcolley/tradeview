using DevelopmentInProgress.TradeView.Wpf.Common.Command;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel
{
    public class SymbolsViewModel : ExchangeViewModel
    {
        private readonly UserAccount userAccount;
        private bool isLoadingSymbols;
        private bool disposed;

        public SymbolsViewModel(IWpfExchangeService exchangeService, UserAccount userAccount, ILoggerFacade logger)
            : base(exchangeService, logger)
        {
            this.userAccount = userAccount;

            UpdatePreferencesCommand = new ViewModelCommand(UpdatePreferences);
            ShowFavouritesCommand = new ViewModelCommand(ShowFavourites);

            Symbols = new List<Symbol>();

            // necessary to get symbols on fire and forget 
            // so it doesn't block the dialog from showing.
            GetSymbols().FireAndForget();
        }

        public ICommand UpdatePreferencesCommand { get; set; }
        public ICommand ShowFavouritesCommand { get; set; }

        public List<Symbol> Symbols { get; }

        public string Exchange
        {
            get { return userAccount.Exchange.ToString(); }
        }

        public bool IsLoadingSymbols
        {
            get { return isLoadingSymbols; }
            set
            {
                if (isLoadingSymbols != value)
                {
                    isLoadingSymbols = value;
                    OnPropertyChanged(nameof(IsLoadingSymbols));
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // dispose stuff...
            }

            disposed = true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "General exceptions are logged and displayed to the user in a dialog box.")]
        private async Task GetSymbols()
        {
            IsLoadingSymbols = true;

            try
            {
                var results = await ExchangeService.GetSymbolsAsync(userAccount.Exchange, new CancellationToken()).ConfigureAwait(true);

                Func<Symbol, string, Symbol> f = ((s, p) =>
                {
                    s.IsFavourite = true;
                    return s;
                });

                (from s in results join p in userAccount.Preferences.FavouriteSymbols on s.ExchangeSymbol equals p select f(s, p)).ToList();

                Symbols.Clear();
                Symbols.AddRange(results);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), Category.Exception, Priority.Low);
                Dialog.ShowException(ex);
            }
            finally
            {
                IsLoadingSymbols = false;
            }
        }

        private void UpdatePreferences(object arg)
        {
            if(arg is Symbol symbol)
            {
                if(symbol.IsFavourite)
                {
                    if(!userAccount.Preferences.FavouriteSymbols.Contains(symbol.ExchangeSymbol))
                    {
                        userAccount.Preferences.FavouriteSymbols.Add(symbol.ExchangeSymbol);
                    }
                }
                else
                {
                    if (userAccount.Preferences.FavouriteSymbols.Contains(symbol.ExchangeSymbol))
                    {
                        userAccount.Preferences.FavouriteSymbols.Remove(symbol.ExchangeSymbol);
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "General exceptions are logged and displayed to the user in a dialog box.")]
        private async void ShowFavourites(object arg)
        {
            if (arg != null && arg is bool showFavourites)
            {
                IsLoadingSymbols = true;

                try
                {
                    await OnShowFavourites(showFavourites).ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.ToString(), Category.Exception, Priority.Low);
                    Dialog.ShowException(ex);
                }
                finally
                {
                    IsLoadingSymbols = false;
                }
            }
        }

        private Task OnShowFavourites(bool showFavourites)
        {
            return Task.Run(() =>
            {
                if (showFavourites)
                {
                    Symbols.ForEach(s => s.IsVisible = s.IsFavourite);
                }
                else
                {
                    Symbols.ForEach(s => s.IsVisible = true);
                }
            });
        }
    }
}
