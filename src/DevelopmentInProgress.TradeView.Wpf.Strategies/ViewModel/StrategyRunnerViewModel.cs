using InterfaceModel = DevelopmentInProgress.TradeView.Interface.Model;
using InterfaceStrategy = DevelopmentInProgress.TradeView.Interface.Strategy;
using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Strategies.Events;
using DevelopmentInProgress.TradeView.Wpf.Strategies.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Reactive.Linq;
using DipSocket.Client;
using System.Net.WebSockets;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies.ViewModel
{
    public class StrategyRunnerViewModel : DocumentViewModel
    {
        private IStrategyService strategyService;
        private IStrategyAssemblyManager strategyAssemblyManager;
        private Strategy strategy;
        private List<Symbol> symbols;
        private bool canRun;
        private bool canMonitor;
        private bool isConnected;
        private bool isConnecting;
        private bool disposed;
        private DipSocketClient socketClient;
        private ObservableCollection<Message> notifications;

        private AccountViewModel accountViewModel;
        private SymbolsViewModel symbolsViewModel;
        private OrdersViewModel ordersViewModel;
        private StrategyParametersViewModel strategyParametersViewModel;
        private StrategyDisplayViewModelBase StrategyDisplayViewModel;

        public StrategyRunnerViewModel(
            ViewModelContext viewModelContext, 
            AccountViewModel accountViewModel, 
            SymbolsViewModel symbolsViewModel, 
            OrdersViewModel ordersViewModel, 
            StrategyParametersViewModel strategyParametersViewModel,
            IStrategyService strategyService, 
            IStrategyAssemblyManager strategyAssemblyManager)
            : base(viewModelContext)
        {
            this.strategyService = strategyService;
            this.strategyAssemblyManager = strategyAssemblyManager;

            AccountViewModel = accountViewModel;
            SymbolsViewModel = symbolsViewModel;
            OrdersViewModel = ordersViewModel;
            StrategyParametersViewModel = strategyParametersViewModel;

            CanRun = false;
            CanMonitor = false;
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
            ObserveParameters();
        }
        
        public event EventHandler<StrategyDisplayEventArgs> OnStrategyDisplay;

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

        public StrategyParametersViewModel StrategyParametersViewModel
        {
            get { return strategyParametersViewModel; }
            private set
            {
                if (strategyParametersViewModel != value)
                {
                    strategyParametersViewModel = value;
                    OnPropertyChanged("StrategyParametersViewModel");
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

        public bool CanRun
        {
            get { return canRun; }
            set
            {
                if (canRun != value)
                {
                    canRun = value;
                    OnPropertyChanged("CanRun");
                }
            }
        }

        public bool CanMonitor
        {
            get { return canMonitor; }
            set
            {
                if (canMonitor != value)
                {
                    canMonitor = value;
                    OnPropertyChanged("CanMonitor");
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

        public Strategy Strategy
        {
            get { return strategy; }
            set
            {
                if (strategy != value)
                {
                    strategy = value;
                    OnPropertyChanged("Strategy");
                }
            }
        }

        protected async override void OnPublished(object data)
        {
            try
            {
                Strategy = await strategyService.GetStrategy(Title);

                strategyAssemblyManager.Activate(Strategy, ViewModelContext.UiDispatcher, Logger);
                StrategyDisplayViewModel = (StrategyDisplayViewModelBase)strategyAssemblyManager.StrategyDisplayViewModel;

                RaiseStrategyDisplayEvent();

                AccountViewModel.Dispatcher = ViewModelContext.UiDispatcher;
                SymbolsViewModel.Dispatcher = ViewModelContext.UiDispatcher;
                OrdersViewModel.Dispatcher = ViewModelContext.UiDispatcher;
                StrategyParametersViewModel.Dispatcher = ViewModelContext.UiDispatcher;

                StrategyParametersViewModel.Strategy = Strategy;

                var account = new Account(new InterfaceModel.AccountInfo { User = new InterfaceModel.User() });

                if (Strategy.StrategySubscriptions.Any())
                {
                    var strategySubscription = Strategy.StrategySubscriptions.First();

                    account.AccountName = strategySubscription.AccountName;
                    account.ApiKey = strategySubscription.ApiKey;
                    account.ApiSecret = strategySubscription.SecretKey;
                    account.ApiPassPhrase = strategySubscription.ApiPassPhrase;
                    account.Exchange = strategySubscription.Exchange;
                }

                await Task.WhenAll(SymbolsViewModel.GetSymbols(Strategy), AccountViewModel.Login(account));

                if (Strategy.StrategySubscriptions.Any())
                {
                    if (!IsConnected)
                    {
                        await SetCommandVisibility(true, false, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"OnPublished {ex.Message}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                ShowMessage(new Message { MessageType = MessageType.Error, Text = $"Strategy load error. Check configuration. {ex.Message}" });
            }
        }

        private async void RunStrategy(object param)
        {
            await RunAsync();
        }

        private async void MonitorStrategy(object param)
        {
            await MonitorAsync();
        }

        private async void Disconnect(object param)
        {
            await SetCommandVisibility(false, true, false);

            await DisconnectSocketAsync();

            await SetCommandVisibility(true, false, false);
        }

        private async void StopStrategy(object param)
        {
            await SetCommandVisibility(false, true, false);

            var strategyParameters = new InterfaceStrategy.StrategyParameters { StrategyName = Strategy.Name };
            var strategyParametersJson = JsonConvert.SerializeObject(strategyParameters);

            await StopAsync(strategyParametersJson);
            
            await SetCommandVisibility(true, false, false);
        }

        private void ClearNotifications(object param)
        {
            Notifications.Clear();
        }

        protected async override void OnDisposing()
        {
            if (disposed)
            {
                return;
            }

            await DisconnectSocketAsync(false);

            AccountViewModel.Dispose();
            SymbolsViewModel.Dispose();
            OrdersViewModel.Dispose();
            StrategyParametersViewModel.Dispose();
            strategyAssemblyManager.Dispose();

            if(StrategyDisplayViewModel != null)
            {
                StrategyDisplayViewModel.Dispose();
            }

            disposed = true;
        }

        private async Task DisconnectSocketAsync(bool writeNotification = true)
        {
            if (socketClient != null)
            {
                try
                {
                    if (socketClient.State.Equals(WebSocketState.Open))
                    {
                        var clientMessage = new DipSocket.Messages.Message
                        {
                            SenderConnectionId = strategyAssemblyManager.Id,
                            Data = strategy.Name,
                            MessageType = DipSocket.Messages.MessageType.UnsubscribeFromChannel
                        };

                        await socketClient.SendMessageAsync(clientMessage);
                    }

                    await DisposeSocketAsync(writeNotification);
                }
                catch (Exception ex)
                {
                    Logger.Log($"DisconnectSocketAsync {ex.Message}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                    if (writeNotification)
                    {
                        ViewModelContext.UiDispatcher.Invoke(() =>
                        {
                            NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"Disconnect - {ex.Message}", TextVerbose = ex.ToString() });
                        });
                    }
                }
            }
        }

        private async Task<bool> IsStrategyRunningAsync()
        {
            try
            {
                var strategyParameters = new InterfaceStrategy.StrategyParameters { StrategyName = Strategy.Name };
                var strategyParametersJson = JsonConvert.SerializeObject(strategyParameters);

                var strategyRunnerClient = new InterfaceStrategy.StrategyRunnerClient();

                var response = await strategyRunnerClient.PostAsync($"{Strategy.StrategyServerUrl}/isstrategyrunning", strategyParametersJson);

                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return content.Equals("YES");
                }
                else
                {
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
                Logger.Log($"IsStrategyRunningAsync {ex.ToString()}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"Unable to connect to the remote server.", TextVerbose = ex.ToString() });
            }

            return false;
        }

        private async Task DisposeSocketAsync(bool writeNotification = true)
        {
            try
            {
                if (socketClient != null)
                {
                    await socketClient.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"DisposeSocketAsync {ex.Message}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                if (writeNotification)
                {
                    ViewModelContext.UiDispatcher.Invoke(() =>
                    {
                        NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"Disconnect - {ex.Message}", TextVerbose = ex.ToString() });
                    });
                }
            }
            finally
            {
                socketClient = null;
                IsConnected = false;
            }
        }

        private async Task RunAsync()
        {
            try
            {
                var result = await MonitorAsync(true);

                if (result)
                {
                    await SetCommandVisibility(false, false, true);

                    var interfaceStrategy = Strategy.ToInterfaceStrategy();
                    var jsonContent = JsonConvert.SerializeObject(interfaceStrategy);

                    var dependencies = strategy.Dependencies.Select(d => d.File);

                    var strategyRunnerClient = new InterfaceStrategy.StrategyRunnerClient();

                    var response = await strategyRunnerClient.PostAsync($"{Strategy.StrategyServerUrl}/runstrategy", jsonContent, dependencies);

                    if(response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        await SetCommandVisibility(false, false, false);

                        await DisconnectSocketAsync();
                    }
                    else
                    {
                        var accountBalances = AccountViewModel.Account.AccountInfo.Balances.ToList();

                        await SetCommandVisibility(false, false, true);
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
                Logger.Log($"RunAsync {ex.Message}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"Run - {ex.Message}", TextVerbose = ex.ToString() });
                await SetCommandVisibility(false, false, false);
                await DisconnectSocketAsync();
            }
        }

        private async Task Update(string strategyParameters)
        {
            try
            {
                if (!IsConnected)
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Warn, Text = $"Not connected to running strategy"});
                    return;
                }

                var strategyRunnerClient = new TradeView.Interface.Strategy.StrategyRunnerClient();

                var response = await strategyRunnerClient.PostAsync($"{Strategy.StrategyServerUrl}/updatestrategy", strategyParameters);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    await SetCommandVisibility(false, false, false);

                    await DisconnectSocketAsync();
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
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
                Logger.Log($"RunAsync {ex.Message}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"Update - {ex.Message}", TextVerbose = ex.ToString() });
                await SetCommandVisibility(false, false, false);
                await DisconnectSocketAsync();
            }
        }

        private async Task StopAsync(string strategyParameters)
        {
            try
            {
                var strategyRunnerClient = new TradeView.Interface.Strategy.StrategyRunnerClient();

                var response = await strategyRunnerClient.PostAsync($"{Strategy.StrategyServerUrl}/stopstrategy", strategyParameters);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
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

                    await DisconnectSocketAsync();
                }
                else
                {
                    await Task.Delay(500);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"StopAsync {ex.Message}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"Stop - {ex.Message}", TextVerbose = ex.ToString() });
                await SetCommandVisibility(false, false, false);
                await DisconnectSocketAsync();
            }
        }

        private async Task<bool> MonitorAsync(bool isForRun = false)
        {
            if(IsConnected)
            {
                NotificationsAdd(new Message { MessageType = MessageType.Info, Text = $"Already connected to strategy", Timestamp = DateTime.Now });
                return IsConnected;
            }
            
            await SetCommandVisibility(false, true, false);

            Notifications.Clear();

            if(string.IsNullOrWhiteSpace(strategyAssemblyManager.Id))
            {
                throw new Exception("StrategyAssemblyManager has not loaded the strategy assemblies.");
            }

            socketClient = new DipSocketClient($"{Strategy.StrategyServerUrl}/notificationhub", strategyAssemblyManager.Id);

            socketClient.On("Connected", message =>
            {
                ViewModelContext.UiDispatcher.Invoke(() =>
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Info, Text = $"Connected - {message.ToString()}", Timestamp = DateTime.Now });
                });
            });


            socketClient.On("Notification", async (message) =>
            {
                await ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    await OnStrategyNotificationAsync(message);
                });
            });

            socketClient.On("Trade", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    await OnTradeNotificationAsync(message);
                });
            });

            socketClient.On("OrderBook", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    await OnOrderBookNotificationAsync(message);
                });
            });

            socketClient.On("AccountInfo", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    await OnAccountNotificationAsync(message);
                });
            });

            socketClient.On("Candlesticks", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    await OnCandlesticksNotificationAsync(message);
                });
            });

            socketClient.Closed += async (sender, args) =>
            {
                await ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"socketClient.Closed", TextVerbose = args.ToString(), Timestamp = DateTime.Now });

                    await DisposeSocketAsync(false);
                });
            };

            socketClient.Error += async (sender, args) => 
            {
                var ex = args as Exception;
                if(ex.InnerException is TaskCanceledException)
                {
                    return;
                }

                await ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"{args.Message}", TextVerbose = args.ToString(), Timestamp = DateTime.Now });

                    await DisposeSocketAsync(false);
                });
            };

            try
            {
                await socketClient.StartAsync(strategy.Name);

                StrategyDisplayViewModel.IsActive = true;

                if (!isForRun)
                {
                    await SetCommandVisibility(false, false, true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"MonitorAsync {ex.ToString()}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"Monitor - {ex.Message}", TextVerbose=ex.ToString(), Timestamp = DateTime.Now });
                await SetCommandVisibility(false, false, false);
                await DisconnectSocketAsync();
                return false;
            }
        }

        private async Task OnStrategyNotificationAsync(DipSocket.Messages.Message message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<TradeView.Interface.Strategy.StrategyNotification>>(message.Data);

                foreach (var notification in strategyNotifications)
                {
                    NotificationsAdd(notification.GetMessage());

                    if(notification.NotificationLevel.Equals(TradeView.Interface.Strategy.NotificationLevel.DisconnectClient))
                    {
                        await DisconnectSocketAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"OnStrategyNotification {ex.ToString()}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnStrategyNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private async Task OnTradeNotificationAsync(DipSocket.Messages.Message message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<TradeView.Interface.Strategy.StrategyNotification>>(message.Data);

                var orderedStrategyNotifications = strategyNotifications.OrderBy(n => n.Timestamp).ToList();

                await StrategyDisplayViewModel.TradeNotificationsAsync(orderedStrategyNotifications);
            }
            catch (Exception ex)
            {
                Logger.Log($"OnTradeNotification {ex.ToString()}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnTradeNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private async Task OnCandlesticksNotificationAsync(DipSocket.Messages.Message message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<TradeView.Interface.Strategy.StrategyNotification>>(message.Data);

                var orderedStrategyNotifications = strategyNotifications.OrderBy(n => n.Timestamp).ToList();

                await StrategyDisplayViewModel.CandlestickNotificationsAsync(orderedStrategyNotifications);
            }
            catch(Exception ex)
            {
                Logger.Log($"OnCandlesticksNotification {ex.ToString()}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnCandlesticksNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private async Task OnOrderBookNotificationAsync(DipSocket.Messages.Message message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<TradeView.Interface.Strategy.StrategyNotification>>(message.Data);

                var orderedStrategyNotifications = strategyNotifications.OrderBy(n => n.Timestamp).ToList();

                await StrategyDisplayViewModel.OrderNotificationsAsync(orderedStrategyNotifications);
            }
            catch (Exception ex)
            {
                Logger.Log($"OnOrderBookNotification {ex.ToString()}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnOrderBookNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private async Task OnAccountNotificationAsync(DipSocket.Messages.Message message)
        {
            try
            {
                var accountBalances = AccountViewModel.Account.AccountInfo.Balances.ToList();
            }
            catch (Exception ex)
            {
                Logger.Log($"OnAccountNotification {ex.ToString()}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnAccountNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private void ObserveSymbols()
        {
            var symbolsObservable = Observable.FromEventPattern<StrategyListEventArgs>(
                eventHandler => SymbolsViewModel.OnSymbolsNotification += eventHandler,
                eventHandler => SymbolsViewModel.OnSymbolsNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            symbolsObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = args.Message, TextVerbose = args.Exception.ToString() });
                }
                else if (string.IsNullOrWhiteSpace(args.Message) && args.Value.Any())
                {
                    symbols = args.Value;
                    StrategyDisplayViewModel.Symbols = symbols;
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

            accountObservable.Subscribe(async args =>
            {
                if (args.HasException)
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = args.Message, TextVerbose = args.Exception.ToString() });
                }
                else if(args.AccountEventType.Equals(AccountEventType.LoggedIn))
                {
                    await OrdersViewModel.SetAccount(args.Value);
                }
                else if (args.AccountEventType.Equals(AccountEventType.UpdateOrders))
                {
                    await OrdersViewModel.UpdateOrders(args.Value);
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

        private void ObserveParameters()
        {
            var parametersObservable = Observable.FromEventPattern<StrategyEventArgs>(
                eventHandler => StrategyParametersViewModel.OnStrategyParametersNotification += eventHandler,
                eventHandler => StrategyParametersViewModel.OnStrategyParametersNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            parametersObservable.Subscribe(async args =>
            {
                if (args.HasException)
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = args.Message, TextVerbose = args.Exception.ToString() });
                }
                else 
                {
                    if (!string.IsNullOrWhiteSpace(args.Message))
                    {
                        NotificationsAdd(new Message { MessageType = MessageType.Info, Text = args.Message });
                    }

                    await Update(args.Value.Parameters);
                }
            });
        }

        private void NotificationsAdd(Message message)
        {
            Prism.Logging.Category category;

            switch(message.MessageType)
            {
                case MessageType.Error:
                    category = Prism.Logging.Category.Exception;
                    break;
                case MessageType.Warn:
                    category = Prism.Logging.Category.Warn;
                    break;
                default:
                    category = Prism.Logging.Category.Info;
                    break;
            }

            Logger.Log(message.Text, category, Prism.Logging.Priority.Low);

            message.Text = $"{message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss.fff tt")} {message.Text}";
            Notifications.Insert(0, message);
        }

        private async Task SetCommandVisibility(bool canconnect, bool connecting, bool connected)
        {
            try
            {
                if (canconnect)
                {
                    var isRunning = await IsStrategyRunningAsync();

                    CanRun = !isRunning;
                    CanMonitor = isRunning;
                }
                else
                {
                    CanRun = false;
                    CanMonitor = false;
                }

                IsConnecting = connecting;
                IsConnected = connected;

                return;
            }
            catch (Exception ex)
            {
                Logger.Log($"SetCommandVisibility {ex.ToString()}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"SetCommandVisibility - {ex.Message}", TextVerbose = ex.ToString() });
            }

            await SetCommandVisibility(false, false, false);
        }

        private void RaiseStrategyDisplayEvent()
        {
            var onStrategyDisplay = OnStrategyDisplay;
            onStrategyDisplay?.Invoke(this, new StrategyDisplayEventArgs { StrategyAssemblyManager = strategyAssemblyManager });
        }
    }
}
