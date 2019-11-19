using DevelopmentInProgress.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Controls.Messaging;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using System.Reactive.Linq;
using DevelopmentInProgress.Wpf.Trading.Events;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Logging;
using DevelopmentInProgress.Wpf.Common.Events;
using DevelopmentInProgress.Wpf.Common.ViewModel;
using DevelopmentInProgress.Wpf.Common.Chart;
using Newtonsoft.Json;
using DevelopmentInProgress.Wpf.Common.Helpers;

namespace DevelopmentInProgress.Wpf.Trading.ViewModel
{
    public class TradingViewModel : DocumentViewModel
    {
        private IOrderBookHelperFactory orderBookHelperFactory;
        private IWpfExchangeService exchangeService;
        private IAccountsService accountsService;
        private IChartHelper chartHelper;
        private SymbolViewModel selectedSymbol;
        private AccountViewModel accountViewModel;
        private TradeViewModel tradeViewModel;
        private SymbolsViewModel symbolsViewModel;
        private OrdersViewModel ordersViewModel;
        private UserAccount userAccount;
        private Account account;
        private bool isOpen;
        private bool disposed;

        private ObservableCollection<SymbolViewModel> symbols;

        private IDisposable symbolsObservableSubscription;
        private IDisposable accountObservableSubscription;
        private IDisposable tradeObservableSubscription;
        private IDisposable ordersObservableSubscription;
        private Dictionary<string, IDisposable> symbolObservableSubscriptions;

        public TradingViewModel(ViewModelContext viewModelContext, 
            AccountViewModel accountViewModel, SymbolsViewModel symbolsViewModel,
            TradeViewModel tradeViewModel, OrdersViewModel ordersViewModel,
            IWpfExchangeService exchangeService, IAccountsService accountsService,
            IOrderBookHelperFactory orderBookHelperFactory,
            IChartHelper chartHelper)
            : base(viewModelContext)
        {
            AccountViewModel = accountViewModel;
            SymbolsViewModel = symbolsViewModel;
            TradeViewModel = tradeViewModel;
            OrdersViewModel = ordersViewModel;

            Symbols = new ObservableCollection<SymbolViewModel>();
            symbolObservableSubscriptions = new Dictionary<string, IDisposable>();

            this.exchangeService = exchangeService;
            this.accountsService = accountsService;
            this.orderBookHelperFactory = orderBookHelperFactory;
            this.chartHelper = chartHelper;

            ObserveSymbols();
            ObserveAccount();
            ObserveTrade();
            ObserveOrders();

            CloseCommand = new ViewModelCommand(Close);
        }

        public ICommand CloseCommand { get; set; }

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

        protected async override void OnPublished(object data)
        {
            if(isOpen)
            {
                return;
            }

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

            Account = new Account(new Interface.AccountInfo { User = new Interface.User() });

            try
            {
                userAccount = await accountsService.GetAccount(Title);
                var json = JsonConvert.SerializeObject(userAccount, Formatting.Indented);
                Logger.Log(json, Category.Info, Priority.Medium);
            }
            catch (Exception ex)
            {
                TradingViewModelException(ex.ToString(), ex);
            }

            if (userAccount != null
                && userAccount.Preferences != null)
            {
                if (!string.IsNullOrWhiteSpace(userAccount.ApiKey))
                {
                    Account.ApiKey = userAccount.ApiKey;
                    Account.ApiSecret = userAccount.ApiSecret;
                    Account.ApiPassPhrase = userAccount.ApiPassPhrase;
                    Account.Exchange = userAccount.Exchange;
                }
            }

            SymbolsViewModel.SetAccount(userAccount);
            AccountViewModel.SetAccount(Account);

            isOpen = true;

            IsBusy = false;
        }

        protected async override void SaveDocument()
        {
            base.SaveDocument();

            userAccount.ApiKey = Account.ApiKey;
            userAccount.ApiSecret = Account.AccountInfo.User.ApiSecret;
            userAccount.Preferences = new Preferences();
            if(SymbolsViewModel != null)
            {
                userAccount.Preferences.ShowFavourites = SymbolsViewModel.ShowFavourites;
                userAccount.Preferences.FavouriteSymbols = new ObservableCollection<string>((from s in SymbolsViewModel.Symbols where s.IsFavourite select s.Name).ToList());
            }
            
            if(SelectedSymbol != null
                && SelectedSymbol.Symbol != null)
            {
                userAccount.Preferences.SelectedSymbol = SelectedSymbol.Symbol.Name;
            }

            try
            {
                await accountsService.SaveAccount(userAccount);
            }
            catch (Exception ex)
            {
                TradingViewModelException(ex.ToString(), ex);
            }
        }
        
        public void Close(object param)
        {
            var symbol = param as SymbolViewModel;
            if(symbol != null)
            {
                Symbols.Remove(symbol);

                IDisposable subscription;
                if(symbolObservableSubscriptions.TryGetValue(symbol.Symbol.Name, out subscription))
                {
                    subscription.Dispose();
                }

                symbolObservableSubscriptions.Remove(symbol.Symbol.Name);

                symbol.Dispose();
            }
        }

        protected override void OnDisposing()
        {
            if (disposed)
            {
                return;
            }

            if (AccountViewModel != null)
            {
                accountObservableSubscription.Dispose();
                AccountViewModel.Dispose();
            }

            foreach(var subscription in symbolObservableSubscriptions.Values)
            {
                subscription.Dispose();
            }

            foreach(var symbol in Symbols)
            {
                symbol.Dispose();
            }
            
            if (SymbolsViewModel != null)
            {
                symbolsObservableSubscription.Dispose();
                SymbolsViewModel.Dispose();
            }
            
            if(TradeViewModel != null)
            {
                tradeObservableSubscription.Dispose();
                TradeViewModel.Dispose();
            }

            if(OrdersViewModel != null)
            {
                ordersObservableSubscription.Dispose();
                OrdersViewModel.Dispose();
            }

            disposed = true;
        }

        private void ObserveSymbols()
        {
            var symbolsObservable = Observable.FromEventPattern<SymbolsEventArgs>(
                eventHandler => SymbolsViewModel.OnSymbolsNotification += eventHandler,
                eventHandler => SymbolsViewModel.OnSymbolsNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            symbolsObservableSubscription = symbolsObservable.Subscribe(async (args) =>
            {
                if (args.HasException)
                {
                    TradingViewModelException(args);
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
                        symbol = new SymbolViewModel(userAccount.Exchange, exchangeService, chartHelper,
                            orderBookHelperFactory.GetOrderBookHelper(userAccount.Exchange), userAccount.Preferences, Logger);
                        symbol.Dispatcher = ViewModelContext.UiDispatcher;
                        Symbols.Add(symbol);
                        SelectedSymbol = symbol;

                        try
                        {
                            await symbol.SetSymbol(args.Value);
                            ObserveSymbol(symbol);
                        }
                        catch (Exception ex)
                        {
                            TradingViewModelException(ex.ToString(), ex);
                        }
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

            accountObservableSubscription = accountObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    TradingViewModelException(args);
                }
                else if (args.AccountEventType.Equals(AccountEventType.LoggedIn)
                        || args.AccountEventType.Equals(AccountEventType.LoggedOut))
                {
                    TradeViewModel.SetAccount(args.Value);
                    OrdersViewModel.SetAccount(args.Value).FireAndForget();
                }
                else if (args.AccountEventType.Equals(AccountEventType.UpdateOrders))
                {
                    TradeViewModel.Touch();
                    OrdersViewModel.UpdateOrders(args.Value).FireAndForget();
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

            var symbolObservableSubscription =symbolObservable.Subscribe(args =>
            {
                if(args.HasException)
                {
                    TradingViewModelException(args);
                }
            });

            symbolObservableSubscriptions.Add(symbol.Symbol.Name, symbolObservableSubscription);
        }

        private void ObserveTrade()
        {
            var tradeObservable = Observable.FromEventPattern<TradeEventArgs>(
                eventHandler => TradeViewModel.OnTradeNotification += eventHandler,
                eventHandler => TradeViewModel.OnTradeNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            tradeObservableSubscription = tradeObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    TradingViewModelException(args);
                }
            });
        }

        private void ObserveOrders()
        {            
            var ordersObservable = Observable.FromEventPattern<OrdersEventArgs>(
                eventHandler => OrdersViewModel.OnOrdersNotification += eventHandler,
                eventHandler => OrdersViewModel.OnOrdersNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            ordersObservableSubscription = ordersObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    TradingViewModelException(args);
                }
            });
        }

        private void TradingViewModelException<T>(EventArgsBase<T> eventArgs)
        {
            if (eventArgs.Exception != null)
            {
                TradingViewModelException(eventArgs.Message, eventArgs.Exception);
            }
            else
            {
                Logger.Log(eventArgs.Message, Category.Exception, Priority.High);
            }
        }

        private void TradingViewModelException(string message, Exception ex)
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