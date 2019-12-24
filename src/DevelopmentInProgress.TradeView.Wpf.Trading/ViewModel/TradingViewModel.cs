using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DevelopmentInProgress.TradeView.Wpf.Trading.Events;
using Prism.Logging;
using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Common.Chart;
using Newtonsoft.Json;
using DevelopmentInProgress.TradeView.Wpf.Common.Helpers;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.ViewModel
{
    public class TradingViewModel : DocumentViewModel
    {
        private IOrderBookHelperFactory orderBookHelperFactory;
        private ITradeHelperFactory tradeHelperFactory;
        private IWpfExchangeService exchangeService;
        private IAccountsService accountsService;
        private IChartHelper chartHelper;
        private SymbolViewModel symbolViewModel;
        private AccountViewModel accountViewModel;
        private TradeViewModel tradeViewModel;
        private SymbolsViewModel symbolsViewModel;
        private OrdersViewModel ordersViewModel;
        private UserAccount userAccount;
        private Account account;
        private Symbol symbol;
        private bool isOpen;
        private bool disposed;

        private IDisposable symbolsObservableSubscription;
        private IDisposable symbolObservableSubscription;
        private IDisposable accountObservableSubscription;
        private IDisposable tradeObservableSubscription;
        private IDisposable ordersObservableSubscription;

        public TradingViewModel(ViewModelContext viewModelContext, 
            AccountViewModel accountViewModel, SymbolsViewModel symbolsViewModel,
            TradeViewModel tradeViewModel, OrdersViewModel ordersViewModel,
            IWpfExchangeService exchangeService, IAccountsService accountsService,
            IOrderBookHelperFactory orderBookHelperFactory,
            ITradeHelperFactory tradeHelperFactory,
            IChartHelper chartHelper)
            : base(viewModelContext)
        {
            AccountViewModel = accountViewModel;
            SymbolsViewModel = symbolsViewModel;
            TradeViewModel = tradeViewModel;
            OrdersViewModel = ordersViewModel;

            this.exchangeService = exchangeService;
            this.accountsService = accountsService;
            this.orderBookHelperFactory = orderBookHelperFactory;
            this.tradeHelperFactory = tradeHelperFactory;
            this.chartHelper = chartHelper;

            ObserveSymbols();
            ObserveAccount();
            ObserveTrade();
            ObserveOrders();
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

            Account = new Account(new Interface.Model.AccountInfo { User = new Interface.Model.User() });

            try
            {
                userAccount = await accountsService.GetAccountAsync(Title);
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
                    Account.AccountName = userAccount.AccountName;
                    Account.ApiKey = userAccount.ApiKey;
                    Account.ApiSecret = userAccount.ApiSecret;
                    Account.ApiPassPhrase = userAccount.ApiPassPhrase;
                    Account.Exchange = userAccount.Exchange;
                }
            }

            await Task.WhenAll(SymbolsViewModel.SetAccount(userAccount), AccountViewModel.SetAccount(Account));

            isOpen = true;
            IsBusy = false;
        }

        protected override void OnDisposing()
        {
            if (disposed)
            {
                return;
            }

            symbolsObservableSubscription?.Dispose();
            tradeObservableSubscription?.Dispose();
            ordersObservableSubscription?.Dispose();
            accountObservableSubscription.Dispose();
            symbolObservableSubscription?.Dispose();

            AccountViewModel.Dispose();
            SymbolsViewModel?.Dispose();
            TradeViewModel?.Dispose();
            OrdersViewModel?.Dispose();
            SymbolViewModel?.Dispose();

            disposed = true;
        }

        public async override void OnActiveChanged(bool isActive)
        {
            try
            {
                if (isActive)
                {
                    var openDocuments = new FindDocumentViewModel { Module = "Trading" };

                    OnGetViewModels(openDocuments);

                    var tradingViewModels = openDocuments.ViewModels.OfType<TradingViewModel>()
                        .Where(d => d.SymbolViewModel != null && d.SymbolViewModel.IsActive).ToList();

                    foreach (var tradingViewModel in tradingViewModels)
                    {
                        tradingViewModel.DisposeSymbolViewModel();
                    }

                    await LoadSymbolViewModel();
                }
                else
                {
                    DisposeSymbolViewModel();
                }
            }
            catch (Exception ex)
            {
                TradingViewModelException(ex.ToString(), ex);
            }
        }

        private void DisposeSymbolViewModel()
        {
            if (SymbolViewModel != null)
            {
                SymbolViewModel.Dispose();
                SymbolViewModel = null;
            }

            if (symbolObservableSubscription != null)
            {
                symbolObservableSubscription.Dispose();
                symbolObservableSubscription = null;
            }
        }

        private async Task LoadSymbolViewModel()
        {
            if (symbol == null)
            {
                return;
            }

            SymbolViewModel = new SymbolViewModel(
                userAccount.Exchange, exchangeService, chartHelper,
                orderBookHelperFactory.GetOrderBookHelper(userAccount.Exchange),
                tradeHelperFactory.GetTradeHelper(userAccount.Exchange),
                userAccount.Preferences, Logger);

            SymbolViewModel.Dispatcher = ViewModelContext.UiDispatcher;

            ObserveSymbol(SymbolViewModel);

            try
            {
                await SymbolViewModel.SetSymbol(symbol);

                SymbolViewModel.IsActive = true;
            }
            catch (Exception ex)
            {
                TradingViewModelException(ex.ToString(), ex);
            }
        }

        private void ObserveSymbols()
        {
            var symbolsObservable = Observable.FromEventPattern<SymbolsEventArgs>(
                eventHandler => SymbolsViewModel.OnSymbolsNotification += eventHandler,
                eventHandler => SymbolsViewModel.OnSymbolsNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            symbolsObservableSubscription = symbolsObservable.Subscribe(async args =>
            {
                if (args.HasException)
                {
                    TradingViewModelException(args);
                }
                else if (args.Value != null)
                {
                    symbol = args.Value;
                    await LoadSymbolViewModel();
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

            accountObservableSubscription = accountObservable.Subscribe(async args =>
            {
                if (args.HasException)
                {
                    TradingViewModelException(args);
                }
                else if (args.AccountEventType.Equals(AccountEventType.LoggedIn)
                        || args.AccountEventType.Equals(AccountEventType.LoggedOut))
                {
                    TradeViewModel.SetAccount(args.Value);
                    await OrdersViewModel.SetAccount(args.Value);
                }
                else if (args.AccountEventType.Equals(AccountEventType.UpdateOrders))
                {
                    TradeViewModel.Touch();
                    await OrdersViewModel.UpdateOrders(args.Value);
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

            symbolObservableSubscription = symbolObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    TradingViewModelException(args);
                }
            });
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