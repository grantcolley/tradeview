using DevelopmentInProgress.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Trading.Events;
using DevelopmentInProgress.Wpf.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevelopmentInProgress.Wpf.Common.ViewModel;
using Prism.Logging;

namespace DevelopmentInProgress.Wpf.Configuration.ViewModel
{
    public class SymbolsViewModel : ExchangeViewModel
    {
        private List<Symbol> symbols;
        private UserAccount userAccount;
        private bool showFavourites;
        private bool isLoadingSymbols;
        private bool disposed;

        public SymbolsViewModel(IWpfExchangeService exchangeService, UserAccount userAccount, ILoggerFacade logger)
            : base(exchangeService, logger)
        {
            this.userAccount = userAccount;

            GetSymbols().FireAndForget();
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


        private async Task GetSymbols()
        {
            IsLoadingSymbols = true;

            //try
            //{
                //var results = await symbolsCache.GetSymbols(AccountPreferences.Preferences.FavouriteSymbols);

                //Symbols = new List<Symbol>(results);

                // set favourite flag to indicate visibility

            //}
            //catch(Exception ex)
            //{
            //    OnException("SymbolsViewModel.GetSymbols", ex);
            //}

            IsLoadingSymbols = false;
        }
    }
}
