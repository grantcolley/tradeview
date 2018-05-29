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
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class TradingViewModel : DocumentViewModel
    {
        private IExchangeService exchangeService;
        private IPersonaliseService personaliseService;
        private SymbolViewModel selectedSymbol;
        private AccountViewModel accountViewModel;
        private TradeViewModel tradeViewModel;
        private SymbolsViewModel symbolsViewModel;
        private OrdersViewModel ordersViewModel;
        private StrategyViewModel strategyViewModel;
        private User user;
        private Account account;

        public ICommand CloseCommand { get; set; }

        private ObservableCollection<SymbolViewModel> symbols;

        public TradingViewModel(ViewModelContext viewModelContext, 
            AccountViewModel accountViewModel, SymbolsViewModel symbolsViewModel,
            TradeViewModel tradeViewModel, OrdersViewModel ordersViewModel, StrategyViewModel strategyViewModel,
            IExchangeService exchangeService, IPersonaliseService personaliseService)
            : base(viewModelContext)
        {
            AccountViewModel = accountViewModel;
            SymbolsViewModel = symbolsViewModel;
            TradeViewModel = tradeViewModel;
            OrdersViewModel = ordersViewModel;
            StrategyViewModel = strategyViewModel;

            Symbols = new ObservableCollection<SymbolViewModel>();

            this.exchangeService = exchangeService;
            this.personaliseService = personaliseService;

            ObserveSymbols();
            ObserveAccount();
            ObserveTrade();
            ObserveOrders();

            CloseCommand = new ViewModelCommand(Close);
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

        public ObservableCollection<SymbolViewModel> Symbols
        {
            get { return symbols; }
            private set
            {
                if (symbols != value)
                {
                    symbols = value;
                    OnPropertyChanged("Symbols");
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
                    OnPropertyChanged("Account");
                }
            }
        }

        public SymbolViewModel SelectedSymbol
        {
            get { return selectedSymbol; }
            set
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
                    Account.ApiSecret = user.ApiSecret;
                }
            }

            SymbolsViewModel.SetUser(user);
            AccountViewModel.SetAccount(account);

            IsBusy = false;
        }

        protected async override void SaveDocument()
        {
            base.SaveDocument();

            user.ApiKey = Account.ApiKey;
            user.ApiSecret = Account.AccountInfo.User.ApiSecret;
            user.Preferences = new Preferences();
            if(SymbolsViewModel != null)
            {
                user.Preferences.ShowFavourites = SymbolsViewModel.ShowFavourites;
                user.Preferences.FavouriteSymbols = (from s in SymbolsViewModel.Symbols where s.IsFavourite select s.Name).ToList();
            }
            
            if(SelectedSymbol != null
                && SelectedSymbol.Symbol != null)
            {
                user.Preferences.SelectedSymbol = SelectedSymbol.Symbol.Name;
            }

            await personaliseService.SavePreferences(user);
        }
        
        public async void Close(object param)
        {
            var symbol = param as SymbolViewModel;
            if(symbol != null)
            {
                symbol.Dispose();
                Symbols.Remove(symbol);
            }
        }

        protected override void OnDisposing()
        {
            base.OnDisposing();

            if(AccountViewModel != null)
            {
                AccountViewModel.Dispose();
            }

            foreach(var symbol in Symbols)
            {
                symbol.Dispose();
            }

            if (SymbolsViewModel != null)
            {
                SymbolsViewModel.Dispose();
            }

            if(TradeViewModel != null)
            {
                TradeViewModel.Dispose();
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
                    TradeViewModelException(args);
                }
                else if (args.Value != null)
                {
                    var symbol = Symbols.FirstOrDefault(s => s.Symbol.Name.Equals(args.Value.Name));
                    if (symbol != null)
                    {
                        SelectedSymbol = symbol;
                    }
                    else
                    {
                        symbol = new SymbolViewModel(exchangeService);
                        symbol.Dispatcher = ViewModelContext.UiDispatcher;
                        ObserveSymbol(symbol);
                        Symbols.Add(symbol);
                        SelectedSymbol = symbol;
                        await symbol.SetSymbol(args.Value);
                    }
                    
                }
                else if (args.Symbols != null)
                {
                    TradeViewModel.SetSymbols(args.Symbols);
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
                    TradeViewModelException(args);
                }
                else if (args.AccountEventType.Equals(AccountEventType.LoggedIn)
                        || args.AccountEventType.Equals(AccountEventType.LoggedOut))
                {
                    TradeViewModel.SetAccount(args.Value);
                    OrdersViewModel.SetAccount(args.Value);
                }
                else if (args.AccountEventType.Equals(AccountEventType.UpdateOrders))
                {
                    TradeViewModel.Touch();
                    OrdersViewModel.UpdateOrders(args.Value);
                }
                else if (args.AccountEventType.Equals(AccountEventType.SelectedAsset))
                {
                    TradeViewModel.SetSymbol(args.SelectedAsset);
                }
            });
        }

        private void ObserveSymbol(SymbolViewModel symbol)
        {
            var symbolObservable = Observable.FromEventPattern<SymbolEventArgs>(
                eventHandler => symbol.OnSymbolNotification += eventHandler,
                eventHandler => symbol.OnSymbolNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            symbolObservable.Subscribe(args =>
            {
                if(args.HasException)
                {
                    TradeViewModelException(args);
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
                    TradeViewModelException(args);
                }
            });
        }

        private void ObserveOrders()
        {
            var ordersObservable = Observable.FromEventPattern<OrdersEventArgs>(
                eventHandler => OrdersViewModel.OnOrdersNotification += eventHandler,
                eventHandler => OrdersViewModel.OnOrdersNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            ordersObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    TradeViewModelException(args);
                }
            });
        }

        private void TradeViewModelException<T>(EventArgsBase<T> eventArgs)
        {
            if (eventArgs.Exception != null)
            {
                TradeViewModelException(eventArgs.Message, eventArgs.Exception);
            }
            else
            {
                Logger.Log(eventArgs.Message, Category.Exception, Priority.High);
            }
        }

        private void TradeViewModelException(string message, Exception ex)
        {
            Logger.Log(message, Category.Exception, Priority.High);

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