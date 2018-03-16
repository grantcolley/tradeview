using DevelopmentInProgress.Wpf.Controls.Messaging;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.MarketView.Interfaces;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Personalise;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Logging;
using System.Linq;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class TradingViewModel : DocumentViewModel, ISelectedSymbol
    {
        private IExchangeService exchangeService;
        private IPersonaliseService personaliseService;
        private Symbol selectedSymbol;
        private SymbolViewModel symbolViewModel;
        private AccountViewModel accountViewModel;
        private TradeViewModel tradeViewModel;
        private SymbolsViewModel symbolsViewModel;
        private User user;
        private Account account;
        
        public TradingViewModel(ViewModelContext viewModelContext, 
            AccountViewModel accountViewModel, SymbolsViewModel symbolsViewModel,
            IExchangeService exchangeService, IPersonaliseService personaliseService)
            : base(viewModelContext)
        {
            AccountViewModel = accountViewModel;
            SymbolsViewModel = symbolsViewModel;

            this.exchangeService = exchangeService;
            this.personaliseService = personaliseService;
        }

        public SymbolsViewModel SymbolsViewModel
        {
            get { return symbolsViewModel; }
            set
            {
                if (symbolsViewModel != value)
                {
                    symbolsViewModel = value;
                    OnPropertyChanged("SymbolsViewModel");
                }
            }
        }

        public Account Account
        {
            get { return account; }
            private set
            {
                if (account != value)
                {
                    account = value;
                    AccountViewModel.Account = account;
                    OnPropertyChanged("Account");
                }
            }
        }

        public AccountViewModel AccountViewModel
        {
            get { return accountViewModel; }
            set
            {
                if (accountViewModel != value)
                {
                    accountViewModel = value;
                    OnPropertyChanged("AccountViewModel");
                }
            }
        }

        public TradeViewModel TradeViewModel
        {
            get { return tradeViewModel; }
            set
            {
                if (tradeViewModel != value)
                {
                    tradeViewModel = value;
                    OnPropertyChanged("TradeViewModel");
                }
            }
        }

        public Symbol SelectedSymbol
        {
            get { return selectedSymbol; }
            set
            {
                if(selectedSymbol != value)
                {
                    selectedSymbol = value;

                    if(selectedSymbol != null)
                    {
                        SymbolViewModel = new SymbolViewModel(selectedSymbol, exchangeService, TradeViewModelException);
                    }
                    else
                    {
                        SymbolViewModel = null;
                    }

                    OnPropertyChanged("SelectedSymbol");
                    OnPropertyChanged("HasSelectedSymbol");
                }
            }
        }

        public bool HasSelectedSymbol => SelectedSymbol != null ? true : false;

        public SymbolViewModel SymbolViewModel
        {
            get { return symbolViewModel; }
            set
            {
                if (symbolViewModel != value)
                {
                    if(symbolViewModel != null)
                    {
                        symbolViewModel.Dispose();
                    }

                    symbolViewModel = value;
                    OnPropertyChanged("SymbolViewModel");
                }
            }
        }

        protected override async void OnPublished(object data)
        {
            IsBusy = true;

            if(Messages != null
                && Messages.Any())
            {
                ClearMessages();
            }

            Account = new Account(new Interface.AccountInfo { User = new Interface.User() });
            
            user = await personaliseService.GetPreferencesAsync();

            if (user != null
                && user.Preferences != null)
            {
                if (!string.IsNullOrWhiteSpace(user.ApiKey))
                {
                    Account.ApiKey = user.ApiKey;
                }
            }

            SymbolsViewModel.User = user;

            TradeViewModel = new TradeViewModel(Account, exchangeService, TradeViewModelException);

            IsBusy = false;
        }

        protected override void SaveDocument()
        {
            base.SaveDocument();

            user.ApiKey = Account.ApiKey;
            user.Preferences = new Preferences();
            if(SymbolsViewModel != null)
            {
                user.Preferences.ShowFavourites = SymbolsViewModel.ShowFavourites;
                user.Preferences.FavouriteSymbols = (from s in SymbolsViewModel.Symbols where s.IsFavourite select s.Name).ToList();
            }
            
            personaliseService.SavePreferences(user);
        }

        protected override void OnDisposing()
        {
            base.OnDisposing();

            if(AccountViewModel != null)
            {
                AccountViewModel.Dispose();
            }

            if (SymbolViewModel != null)
            {
                SymbolViewModel.Dispose();
            }

            if (SymbolsViewModel != null)
            {
                SymbolsViewModel.Dispose();
            }

            if(tradeViewModel != null)
            {
                tradeViewModel.Dispose();
            }
        }

        private void TradeViewModelException(Exception e)
        {
            var exceptions = new List<Message>();
            if (e is AggregateException)
            {
                foreach(Exception ex in ((AggregateException)e).InnerExceptions)
                {
                    Logger.Log(ex.ToString(), Category.Exception, Priority.High);
                    exceptions.Add(new Message { MessageType = MessageType.Error, Text = ex.Message, TextVerbose = ex.StackTrace });
                }
            }
            else
            {
                Logger.Log(e.ToString(), Category.Exception, Priority.High);
                exceptions.Add(new Message { MessageType = MessageType.Error, Text = e.Message, TextVerbose = e.StackTrace });
            }

            ShowMessages(exceptions);
        }

        public void Notify(Symbol symbol)
        {
            SelectedSymbol = symbol;
        }
    }
}