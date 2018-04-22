using DevelopmentInProgress.Wpf.Controls.Messaging;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Personalise;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Logging;
using System.Linq;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using System.Reactive.Linq;
using DevelopmentInProgress.Wpf.MarketView.Events;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class TradingViewModel : DocumentViewModel
    {
        private IExchangeService exchangeService;
        private IPersonaliseService personaliseService;
        private Symbol selectedSymbol;
        private SymbolViewModel symbolViewModel;
        private AccountViewModel accountViewModel;
        private TradeViewModel tradeViewModel;
        private SymbolsViewModel symbolsViewModel;
        private OrdersViewModel ordersViewModel;
        private StrategyViewModel strategyViewModel;
        private User user;
        private Account account;
        
        public TradingViewModel(ViewModelContext viewModelContext, 
            AccountViewModel accountViewModel, SymbolsViewModel symbolsViewModel,
            TradeViewModel tradeViewModel, SymbolViewModel symbolViewModel,
            OrdersViewModel ordersViewModel, StrategyViewModel strategyViewModel,
            IExchangeService exchangeService, IPersonaliseService personaliseService)
            : base(viewModelContext)
        {
            AccountViewModel = accountViewModel;
            SymbolsViewModel = symbolsViewModel;
            TradeViewModel = tradeViewModel;
            SymbolViewModel = symbolViewModel;
            OrdersViewModel = ordersViewModel;
            StrategyViewModel = strategyViewModel;

            this.exchangeService = exchangeService;
            this.personaliseService = personaliseService;

            ObserveSymbols();
            ObserveAccount();
            ObserveSymbol();
            ObserveTrade();
        }

        public AccountViewModel AccountViewModel
        {
            get { return accountViewModel; }
            private set
            {
                if (accountViewModel != value)
                {
                    accountViewModel = value;
                    OnPropertyChanged("AccountViewModel");
                }
            }
        }

        public SymbolsViewModel SymbolsViewModel
        {
            get { return symbolsViewModel; }
            private set
            {
                if (symbolsViewModel != value)
                {
                    symbolsViewModel = value;
                    OnPropertyChanged("SymbolsViewModel");
                }
            }
        }

        public TradeViewModel TradeViewModel
        {
            get { return tradeViewModel; }
            private set
            {
                if (tradeViewModel != value)
                {
                    tradeViewModel = value;
                    OnPropertyChanged("TradeViewModel");
                }
            }
        }

        public SymbolViewModel SymbolViewModel
        {
            get { return symbolViewModel; }
            private set
            {
                if (symbolViewModel != value)
                {
                    symbolViewModel = value;
                    OnPropertyChanged("SymbolViewModel");
                }
            }
        }

        public OrdersViewModel OrdersViewModel
        {
            get { return ordersViewModel; }
            private set
            {
                if (ordersViewModel != value)
                {
                    ordersViewModel = value;
                    OnPropertyChanged("OrdersViewModel");
                }
            }
        }

        public StrategyViewModel StrategyViewModel
        {
            get { return strategyViewModel; }
            private set
            {
                if (strategyViewModel != value)
                {
                    strategyViewModel = value;
                    OnPropertyChanged("StrategyViewModel");
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

        public Symbol SelectedSymbol
        {
            get { return selectedSymbol; }
            private set
            {
                if(selectedSymbol != value)
                {
                    selectedSymbol = value;
                    OnPropertyChanged("SelectedSymbol");
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

            accountViewModel.Dispatcher = ViewModelContext.UiDispatcher;
            symbolsViewModel.Dispatcher = ViewModelContext.UiDispatcher;
            tradeViewModel.Dispatcher = ViewModelContext.UiDispatcher;
            symbolViewModel.Dispatcher = ViewModelContext.UiDispatcher;
            ordersViewModel.Dispatcher = ViewModelContext.UiDispatcher;
            strategyViewModel.Dispatcher = ViewModelContext.UiDispatcher;

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
            
            if(SelectedSymbol != null)
            {
                user.Preferences.SelectedSymbol = SelectedSymbol.Name;
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

            if(OrdersViewModel != null)
            {
                OrdersViewModel.Dispose();
            }
        }

        private void ObserveSymbols()
        {
            var symbolsObservable = Observable.FromEventPattern<SymbolsEventArgs>(
                eventHandler => SymbolsViewModel.OnSymbolsNotification += eventHandler,
                eventHandler => SymbolsViewModel.OnSymbolsNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            symbolsObservable.Subscribe(async (args) =>
            {
                if (args.HasException)
                {
                    TradeViewModelException(args.Exception);
                }
                else if(args.Value != null)
                {
                    SelectedSymbol = args.Value;
                    await SymbolViewModel.SetSymbol(selectedSymbol);
                }
                else if (args.Symbols != null)
                {
                    tradeViewModel.SetSymbols(args.Symbols);
                }
            });
        }

        private void ObserveAccount()
        {
            var accountObservable = Observable.FromEventPattern<AccountEventArgs>(
                eventHandler => AccountViewModel.OnAccountNotification += eventHandler,
                eventHandler => AccountViewModel.OnAccountNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            accountObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    TradeViewModelException(args.Exception);
                }
                else if(args.UpdateOrders)
                {
                    OrdersViewModel.UpdateOrders(args.Value);
                }
                else
                {
                    TradeViewModel.SetAccount(args.Value, args.SelectedAsset);
                    OrdersViewModel.SetAccount(args.Value);
                }
            });
        }

        private void ObserveSymbol()
        {
            var symbolObservable = Observable.FromEventPattern<SymbolEventArgs>(
                eventHandler => SymbolViewModel.OnSymbolNotification += eventHandler,
                eventHandler => SymbolViewModel.OnSymbolNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            symbolObservable.Subscribe(args =>
            {
                if(args.HasException)
                {
                    TradeViewModelException(args.Exception);
                }
            });
        }

        private void ObserveTrade()
        {
            var tradeObservable = Observable.FromEventPattern<TradeEventArgs>(
                eventHandler => TradeViewModel.OnTradeNotification += eventHandler,
                eventHandler => TradeViewModel.OnTradeNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            tradeObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    TradeViewModelException(args.Exception);
                }
            });
        }

        private void TradeViewModelException(Exception ex)
        {
            var exceptions = new List<Message>();
            if (ex is AggregateException)
            {
                foreach(Exception e in ((AggregateException)ex).InnerExceptions)
                {
                    Logger.Log(e.ToString(), Category.Exception, Priority.High);
                    exceptions.Add(new Message { MessageType = MessageType.Error, Text = e.Message, TextVerbose = e.StackTrace });
                }
            }
            else
            {
                Logger.Log(ex.ToString(), Category.Exception, Priority.High);
                exceptions.Add(new Message { MessageType = MessageType.Error, Text = ex.Message, TextVerbose = ex.StackTrace });
            }

            ShowMessages(exceptions);
        }
    }
}