using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Controls.Messaging;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.StrategyManager.Extensions;
using DevelopmentInProgress.Wpf.StrategyManager.Model;
using DevelopmentInProgress.Wpf.StrategyManager.Services;
using LiveCharts;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DevelopmentInProgress.Wpf.Common.Extensions;
using DevelopmentInProgress.Wpf.StrategyManager.Events;
using System.Reactive.Linq;
using DevelopmentInProgress.Wpf.Common.ViewModel;
using DevelopmentInProgress.Wpf.Common.Events;

namespace DevelopmentInProgress.Wpf.StrategyManager.ViewModel
{
    public class StrategyViewModel : DocumentViewModel
    {
        private IStrategyService strategyService;
        private Strategy strategy;
        private bool canConnect;
        private bool isConnected;
        private bool isConnecting;
        private HubConnection hubConnection;
        private ObservableCollection<Message> notifications;

        private AccountViewModel accountViewModel;
        private TradesViewModel tradesViewModel;
        private SymbolsViewModel symbolsViewModel;
        private OrdersViewModel ordersViewModel;
        private ChartViewModel chartViewModel;

        public StrategyViewModel(ViewModelContext viewModelContext, AccountViewModel accountViewModel, SymbolsViewModel symbolsViewModel,
            TradesViewModel tradesViewModel, OrdersViewModel ordersViewModel, ChartViewModel chartViewModel, IStrategyService strategyService)
            : base(viewModelContext)
        {
            this.strategyService = strategyService;

            AccountViewModel = accountViewModel;
            SymbolsViewModel = symbolsViewModel;
            TradesViewModel = tradesViewModel;
            OrdersViewModel = ordersViewModel;
            ChartViewModel = chartViewModel;

            CanConnect = true;
            IsConnected = false;
            IsConnecting = false;

            Notifications = new ObservableCollection<Message>();

            RunCommand = new ViewModelCommand(RunStrategy);
            MonitorCommand = new ViewModelCommand(MonitorStrategy);
            DisconnectCommand = new ViewModelCommand(Disconnect);
            StopCommand = new ViewModelCommand(StopStrategy);
            ClearNotificationsCommand = new ViewModelCommand(ClearNotifications);

            ObserveSymbols();
            ObserveAccount();
            ObserveOrders();
        }

        public ICommand RunCommand { get; set; }
        public ICommand MonitorCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand ClearNotificationsCommand { get; set; }

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

        public TradesViewModel TradesViewModel
        {
            get { return tradesViewModel; }
            private set
            {
                if (tradesViewModel != value)
                {
                    tradesViewModel = value;
                    OnPropertyChanged("TradesViewModel");
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

        public ChartViewModel ChartViewModel
        {
            get { return chartViewModel; }
            private set
            {
                if (chartViewModel != value)
                {
                    chartViewModel = value;
                    OnPropertyChanged("ChartViewModel");
                }
            }
        }

        public ObservableCollection<Message> Notifications
        {
            get { return notifications; }
            set
            {
                if (notifications != value)
                {
                    notifications = value;
                    OnPropertyChanged("Notifications");
                }
            }
        }

        public bool CanConnect
        {
            get { return canConnect; }
            set
            {
                if (canConnect != value)
                {
                    canConnect = value;
                    OnPropertyChanged("CanConnect");
                }
            }
        }

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    OnPropertyChanged("IsConnected");
                }
            }
        }

        public bool IsConnecting
        {
            get { return isConnecting; }
            set
            {
                if (isConnecting != value)
                {
                    isConnecting = value;
                    OnPropertyChanged("IsConnecting");
                }
            }
        }

        protected override void OnPublished(object data)
        {
            Strategy = strategyService.GetStrategy(Title);

            AccountViewModel.Dispatcher = ViewModelContext.UiDispatcher;
            SymbolsViewModel.Dispatcher = ViewModelContext.UiDispatcher;
            OrdersViewModel.Dispatcher = ViewModelContext.UiDispatcher;

            SymbolsViewModel.GetSymbols(Strategy).FireAndForget();

            var strategySubscription = Strategy.StrategySubscriptions.First();
            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = strategySubscription.ApiKey,
                ApiSecret = strategySubscription.SecretKey
            };

            AccountViewModel.Login(account).FireAndForget();
        }

        public Strategy Strategy
        {
            get { return strategy; }
            set
            {
                if(strategy!= value)
                {
                    strategy = value;
                    OnPropertyChanged("Strategy");
                }
            }
        }

        private void RunStrategy(object param)
        {
            Run().FireAndForget();
        }

        private void MonitorStrategy(object param)
        {
            Monitor().FireAndForget();
        }

        private void Disconnect(object param)
        {
            Disconnect().FireAndForget();
        }

        private void StopStrategy(object param)
        {

        }

        private void ClearNotifications(object param)
        {
            Notifications.Clear();
        }

        protected async override void OnDisposing()
        {
            Dispose();

            await Disconnect();

            AccountViewModel.Dispose();
            SymbolsViewModel.Dispose();
            OrdersViewModel.Dispose();
            ChartViewModel.Dispose();
        }

        private async Task Disconnect()
        {
            if (hubConnection != null)
            {
                try
                {
                    await hubConnection.DisposeAsync();
                }
                catch (Exception ex)
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"Disconnect - {ex.Message}", TextVerbose = ex.ToString() });
                }
                finally
                {
                    hubConnection = null;
                    IsConnected = false;
                }
            }
        }

        private async Task Run()
        {
            try
            {
                var result = await Monitor(true);

                if (result)
                {
                    SetCommandVisibility(false, false, true);

                    var jsonContent = JsonConvert.SerializeObject(Strategy.GetInterfaceStrategy());
                    var dependencies = strategy.Dependencies.Select(d => d.File);

                    var strategyRunnerClient = new MarketView.Interface.TradeStrategy.StrategyRunnerClient();

                    var response = await strategyRunnerClient.PostAsync($"{Strategy.StrategyServerUrl}/runstrategy", jsonContent, dependencies);

                    if(response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        await Disconnect().ConfigureAwait(false);
                    }

                    var content = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

                    ViewModelContext.UiDispatcher.Invoke(() =>
                    {
                        NotificationsAdd(
                            new Message
                            {
                                MessageType = response.StatusCode == System.Net.HttpStatusCode.OK ? MessageType.Info : MessageType.Error,
                                Text = response.StatusCode.ToString(),
                                TextVerbose = JsonConvert.SerializeObject(content, Formatting.Indented)
                            });
                    });
                }
            }
            catch (Exception ex)
            {
                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"Run - {ex.Message}", TextVerbose = ex.ToString() });
                await Disconnect().ConfigureAwait(false);
            }

            SetCommandVisibility(true, false, false);
        }
        
        private async Task<bool> Monitor(bool isForRun = false)
        {
            if(IsConnected)
            {
                return IsConnected;
            }

            SetCommandVisibility(false, true, false);

            Notifications.Clear();

            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{Strategy.StrategyServerUrl}/notificationhub?strategyname={Strategy.Name}")
                .Build();

            hubConnection.On<object>("Connected", message =>
            {
                ViewModelContext.UiDispatcher.Invoke(() =>
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Info, Text = $"Connected - {message.ToString()}", Timestamp = DateTime.Now });
                });
            });

            hubConnection.On<object>("Notification", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(() =>
                {
                    OnStrategyNotification(message);
                });
            });

            hubConnection.On<object>("Trade", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(() =>
                {
                    OnTradeNotification(message);
                });
            });

            hubConnection.On<object>("OrderBook", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(() =>
                {
                    OnOrderBookNotification(message);
                });
            });

            hubConnection.On<object>("AccountInfo", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(() =>
                {
                    OnAccountNotification(message);
                });
            });

            try
            {
                await hubConnection.StartAsync();

                ChartViewModel.IsActive = true;

                if (!isForRun)
                {
                    SetCommandVisibility(false, false, true);
                }

                return true;
            }
            catch (Exception ex)
            {
                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"Monitor - {ex.Message}", TextVerbose=ex.ToString(), Timestamp = DateTime.Now });
                SetCommandVisibility(true, false, false);
                await Disconnect().ConfigureAwait(false);
                return false;
            }
        }

        private void OnStrategyNotification(object message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<MarketView.Interface.TradeStrategy.StrategyNotification>>(message.ToString());

                foreach (var notification in strategyNotifications)
                {
                    NotificationsAdd(notification.GetMessage());
                }
            }
            catch (Exception ex)
            {
                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnStrategyNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private void OnTradeNotification(object message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<MarketView.Interface.TradeStrategy.StrategyNotification>>(message.ToString());

                foreach (var notification in strategyNotifications)
                {
                    var trades = JsonConvert.DeserializeObject<IEnumerable<Interface.AggregateTrade>>(notification.Message);
                    ChartViewModel.UpdateTrades(trades);
                }
            }
            catch (Exception ex)
            {
                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnStrategyNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private void OnOrderBookNotification(object message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<MarketView.Interface.TradeStrategy.StrategyNotification>>(message.ToString());

                foreach (var notification in strategyNotifications)
                {
                    NotificationsAdd(notification.GetMessage());
                }
            }
            catch (Exception ex)
            {
                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnStrategyNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private void OnAccountNotification(object message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<MarketView.Interface.TradeStrategy.StrategyNotification>>(message.ToString());

                foreach (var notification in strategyNotifications)
                {
                    NotificationsAdd(notification.GetMessage());
                }
            }
            catch (Exception ex)
            {
                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnStrategyNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private void ObserveSymbols()
        {
            var symbolsObservable = Observable.FromEventPattern<StrategyEventArgs>(
                eventHandler => SymbolsViewModel.OnSymbolsNotification += eventHandler,
                eventHandler => SymbolsViewModel.OnSymbolsNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            symbolsObservable.Subscribe(async (args) =>
            {
                if (args.HasException)
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = args.Message, TextVerbose = args.Exception.ToString() });
                }
                else if (string.IsNullOrWhiteSpace(args.Message) && args.Value.Any())
                {
                    ChartViewModel.Symbols = args.Value;
                }
                else if (!string.IsNullOrWhiteSpace(args.Message))
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Info, Text = args.Message });
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
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = args.Message, TextVerbose = args.Exception.ToString() });
                }
                else if(args.AccountEventType.Equals(AccountEventType.SetAccount))
                {
                    OrdersViewModel.SetAccount(args.Value).FireAndForget();
                }
                else if (args.AccountEventType.Equals(AccountEventType.UpdateOrders))
                {
                    OrdersViewModel.UpdateOrders(args.Value).FireAndForget();
                }
                else if (!string.IsNullOrWhiteSpace(args.Message))
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Info, Text = args.Message });
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
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = args.Message, TextVerbose = args.Exception.ToString() });
                }
                else if (!string.IsNullOrWhiteSpace(args.Message))
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Info, Text = args.Message });
                }
            });
        }

        private void NotificationsAdd(Message message)
        {
            message.Text = $"{message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss.fff tt")} {message.Text}";
            Notifications.Add(message);
        }

        private void SetCommandVisibility(bool canconnect, bool connecting, bool connected)
        {
            CanConnect = canconnect;
            IsConnecting = connecting;
            IsConnected = connected;
        }
    }
}
