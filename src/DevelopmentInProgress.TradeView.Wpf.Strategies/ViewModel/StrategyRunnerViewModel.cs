using DevelopmentInProgress.Socket.Client;
using DevelopmentInProgress.TradeView.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Cache;
using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Strategies.Enums;
using DevelopmentInProgress.TradeView.Wpf.Strategies.Events;
using DevelopmentInProgress.TradeView.Wpf.Strategies.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CoreModel = DevelopmentInProgress.TradeView.Core.Model;
using CoreStrategy = DevelopmentInProgress.TradeView.Core.TradeStrategy;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies.ViewModel
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    public class StrategyRunnerViewModel : DocumentViewModel
    {
        private readonly IStrategyService strategyService;
        private readonly IServerMonitorCache serverMonitorCache;
        private readonly IStrategyAssemblyManager strategyAssemblyManager;
        private readonly SemaphoreSlim commandVisibilitySemaphoreSlim = new SemaphoreSlim(1, 1);

        private Strategy strategy;
        private ServerMonitor selectedServer;
        private List<Symbol> symbols;
        private bool canRun;
        private bool canMonitor;
        private bool isConnected;
        private bool isConnecting;
        private bool disposed;
        private SocketClient socketClient;
        private ObservableCollection<ServerMonitor> servers;

        private AccountViewModel accountViewModel;
        private SymbolsViewModel symbolsViewModel;
        private OrdersViewModel ordersViewModel;
        private StrategyParametersViewModel strategyParametersViewModel;
        private StrategyDisplayViewModelBase StrategyDisplayViewModel;

        private IDisposable symbolsSubscription;
        private IDisposable accountSubscription;
        private IDisposable ordersSubscription;
        private IDisposable parametersSubscription;
        private IDisposable strategySubscription;
        private IDisposable serverMonitorCacheSubscription;

        public StrategyRunnerViewModel(
            ViewModelContext viewModelContext, 
            AccountViewModel accountViewModel, 
            SymbolsViewModel symbolsViewModel, 
            OrdersViewModel ordersViewModel, 
            StrategyParametersViewModel strategyParametersViewModel,
            IStrategyService strategyService,
            IServerMonitorCache serverMonitorCache,
            IStrategyAssemblyManager strategyAssemblyManager)
            : base(viewModelContext)
        {
            this.strategyService = strategyService;
            this.serverMonitorCache = serverMonitorCache;
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

            ObserveOrders();
            ObserveSymbols();
            ObserveAccount();
            ObserveParameters();
            ObserveServerMonitorCache();
        }

        public event EventHandler<StrategyDisplayEventArgs> OnStrategyDisplay;

        public ICommand RunCommand { get; set; }
        public ICommand MonitorCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand ClearNotificationsCommand { get; set; }

        public ObservableCollection<Message> Notifications { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Servers must point to cached collection.")]
        public ObservableCollection<ServerMonitor> Servers
        {
            get { return servers; }
            set
            {
                if (servers != value)
                {
                    servers = value;
                    OnPropertyChanged(nameof(Servers));
                }
            }
        }

        public ServerMonitor SelectedServer
        {
            get { return selectedServer; }
            set
            {
                if (selectedServer != value)
                {
                    selectedServer = value;
                    ResetCommandVisibility();
                    OnPropertyChanged(nameof(SelectedServer));
                }
            }
        }

        public AccountViewModel AccountViewModel
        {
            get { return accountViewModel; }
            private set
            {
                if (accountViewModel != value)
                {
                    accountViewModel = value;
                    OnPropertyChanged(nameof(AccountViewModel));
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
                    OnPropertyChanged(nameof(SymbolsViewModel));
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
                    OnPropertyChanged(nameof(OrdersViewModel));
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
                    OnPropertyChanged(nameof(StrategyParametersViewModel));
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
                    OnPropertyChanged(nameof(CanRun));
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
                    OnPropertyChanged(nameof(CanMonitor));
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

                    if(StrategyParametersViewModel != null)
                    {
                        StrategyParametersViewModel.CanPushParameters = isConnected;
                    }

                    OnPropertyChanged(nameof(IsConnected));
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
                    OnPropertyChanged(nameof(IsConnecting));
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
                    OnPropertyChanged(nameof(Strategy));
                }
            }
        }

        protected async override void OnPublished(object data)
        {
            try
            {
                Strategy = await strategyService.GetStrategy(Title).ConfigureAwait(true);

                strategyAssemblyManager.Activate(Strategy, ViewModelContext.UiDispatcher, Logger);
                StrategyDisplayViewModel = (StrategyDisplayViewModelBase)strategyAssemblyManager.StrategyDisplayViewModel;

                ObserveStrategy();

                RaiseStrategyDisplayEvent();

                AccountViewModel.Dispatcher = ViewModelContext.UiDispatcher;
                SymbolsViewModel.Dispatcher = ViewModelContext.UiDispatcher;
                OrdersViewModel.Dispatcher = ViewModelContext.UiDispatcher;
                StrategyParametersViewModel.Dispatcher = ViewModelContext.UiDispatcher;

                StrategyParametersViewModel.Strategy = Strategy;

                var account = new Account(new CoreModel.AccountInfo { User = new CoreModel.User() });

                if (Strategy.StrategySubscriptions.Any())
                {
                    var strategySubscription = Strategy.StrategySubscriptions.First();

                    account.AccountName = strategySubscription.AccountName;
                    account.ApiKey = strategySubscription.ApiKey;
                    account.ApiSecret = strategySubscription.SecretKey;
                    account.ApiPassPhrase = strategySubscription.ApiPassPhrase;
                    account.Exchange = strategySubscription.Exchange;
                }

                await Task.WhenAll(SymbolsViewModel.GetStrategySymbols(Strategy), AccountViewModel.Login(account), GetServerMonitors()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Log($"OnPublished {ex.Message}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                ShowMessage(new Message { MessageType = MessageType.Error, Text = $"Strategy load error. Check configuration. {ex.Message}" });
            }
        }

        private async Task GetServerMonitors()
        {
            try
            {
                Servers = await serverMonitorCache.GetServerMonitorsAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                Logger.Log($"GetServerMonitors {ex.Message}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);
                ShowMessage(new Message { MessageType = MessageType.Error, Text = $"Server load error. {ex.Message}" });
            }
        }

        private async void RunStrategy(object param)
        {
            await RunAsync().ConfigureAwait(false);
        }

        private async void MonitorStrategy(object param)
        {
            await MonitorAsync().ConfigureAwait(false);
        }

        private async void Disconnect(object param)
        {
            await SetCommandVisibility(StrategyRunnerCommandVisibility.Connecting).ConfigureAwait(false);

            await DisconnectSocketAsync().ConfigureAwait(false);

            await SetCommandVisibility(StrategyRunnerCommandVisibility.ServerAvailable).ConfigureAwait(false);
        }

        private async void StopStrategy(object param)
        {
            await SetCommandVisibility(StrategyRunnerCommandVisibility.Connecting).ConfigureAwait(false);

            var strategyParameters = new CoreStrategy.StrategyParameters { StrategyName = Strategy.Name };
            var strategyParametersJson = JsonConvert.SerializeObject(strategyParameters);

            await StopAsync(strategyParametersJson).ConfigureAwait(false);
            
            await SetCommandVisibility(StrategyRunnerCommandVisibility.ServerAvailable).ConfigureAwait(false);
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

            await DisconnectSocketAsync(false).ConfigureAwait(true);

            symbolsSubscription.Dispose();
            accountSubscription.Dispose();
            ordersSubscription.Dispose();
            parametersSubscription.Dispose();

            AccountViewModel.Dispose();
            SymbolsViewModel.Dispose();
            OrdersViewModel.Dispose();
            StrategyParametersViewModel.Dispose();
            serverMonitorCacheSubscription.Dispose();

            if (StrategyDisplayViewModel != null)
            {
                strategySubscription.Dispose();
                StrategyDisplayViewModel.Dispose();
            }

            strategyAssemblyManager.Dispose();

            commandVisibilitySemaphoreSlim.Dispose();

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
                        var clientMessage = new Socket.Messages.Message
                        {
                            SenderConnectionId = strategyAssemblyManager.Id,
                            Data = strategy.Name,
                            MessageType = Socket.Messages.MessageType.UnsubscribeFromChannel
                        };

                        await socketClient.SendMessageAsync(clientMessage).ConfigureAwait(true);
                    }

                    await DisposeSocketAsync(writeNotification).ConfigureAwait(true);
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
                var strategyParameters = new CoreStrategy.StrategyParameters { StrategyName = Strategy.Name };
                var strategyParametersJson = JsonConvert.SerializeObject(strategyParameters);

                var response = await CoreStrategy.StrategyRunnerClient.PostAsync(new Uri($"{SelectedServer.Uri}isstrategyrunning"), strategyParametersJson).ConfigureAwait(false);

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return content == "YES";
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
                Logger.Log($"IsStrategyRunningAsync {ex}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

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
                    await socketClient.DisposeAsync().ConfigureAwait(false);
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
                var result = await MonitorAsync(true).ConfigureAwait(false);

                if (result)
                {
                    var interfaceStrategy = Strategy.ToCoreStrategy();
                    var jsonContent = JsonConvert.SerializeObject(interfaceStrategy);

                    var dependencies = strategy.Dependencies.Select(d => d.File);

                    var response = await CoreStrategy.StrategyRunnerClient.PostAsync(new Uri($"{SelectedServer.Uri}runstrategy"), jsonContent, dependencies).ConfigureAwait(false);

                    if(response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        await SetCommandVisibility(StrategyRunnerCommandVisibility.ServerUnavailable).ConfigureAwait(false);

                        await DisconnectSocketAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        await SetCommandVisibility(StrategyRunnerCommandVisibility.Connected).ConfigureAwait(false);
                    }

                    var content = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

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
                await SetCommandVisibility(StrategyRunnerCommandVisibility.ServerUnavailable).ConfigureAwait(false);
                await DisconnectSocketAsync().ConfigureAwait(false);
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

                var response = await CoreStrategy.StrategyRunnerClient.PostAsync(new Uri($"{SelectedServer.Uri}updatestrategy"), strategyParameters).ConfigureAwait(false);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    await SetCommandVisibility(StrategyRunnerCommandVisibility.ServerUnavailable).ConfigureAwait(false);

                    await DisconnectSocketAsync().ConfigureAwait(false);
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var content = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

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
                await SetCommandVisibility(StrategyRunnerCommandVisibility.ServerUnavailable).ConfigureAwait(false);
                await DisconnectSocketAsync().ConfigureAwait(false);
            }
        }

        private async Task StopAsync(string strategyParameters)
        {
            try
            {
                var response = await CoreStrategy.StrategyRunnerClient.PostAsync(new Uri($"{SelectedServer.Uri}stopstrategy"), strategyParameters).ConfigureAwait(false);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var content = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

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

                    await DisconnectSocketAsync().ConfigureAwait(false);
                }
                else
                {
                    await Task.Delay(500).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"StopAsync {ex.Message}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"Stop - {ex.Message}", TextVerbose = ex.ToString() });
                await SetCommandVisibility(StrategyRunnerCommandVisibility.ServerUnavailable).ConfigureAwait(false);
                await DisconnectSocketAsync().ConfigureAwait(false);
            }
        }

        private async Task<bool> MonitorAsync(bool isForRun = false)
        {
            if(IsConnected)
            {
                NotificationsAdd(new Message { MessageType = MessageType.Info, Text = $"Already connected to strategy", Timestamp = DateTime.Now });
                return IsConnected;
            }

            await SetCommandVisibility(StrategyRunnerCommandVisibility.Connecting).ConfigureAwait(true);

            Notifications.Clear();

            if(string.IsNullOrWhiteSpace(strategyAssemblyManager.Id))
            {
                throw new Exception("StrategyAssemblyManager has not loaded the strategy assemblies.");
            }

            socketClient = new SocketClient(new Uri(SelectedServer.Uri, "notificationhub"), strategyAssemblyManager.Id);

            socketClient.On("Connected", message =>
            {
                ViewModelContext.UiDispatcher.Invoke(() =>
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Info, Text = $"Connected - {message}", Timestamp = DateTime.Now });
                });
            });

            socketClient.On("Notification", async (message) =>
            {
                await ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    await OnStrategyNotificationAsync(message).ConfigureAwait(false);
                }).ConfigureAwait(false);
            });

            socketClient.On("Trade", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    await OnTradeNotificationAsync(message).ConfigureAwait(false);
                });
            });

            socketClient.On("OrderBook", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    await OnOrderBookNotificationAsync(message).ConfigureAwait(false);
                });
            });

            socketClient.On("AccountInfo", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(() =>
                {
                    OnAccountNotification(message);
                });
            });

            socketClient.On("Candlesticks", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    await OnCandlesticksNotificationAsync(message).ConfigureAwait(false);
                });
            });

            socketClient.On("ParameterUpdate", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    await OnParameterUpdateNotificationAsync(message).ConfigureAwait(false);
                });
            });

            socketClient.Closed += async (sender, args) =>
            {
                await ViewModelContext.UiDispatcher.Invoke(async () =>
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"socketClient.Closed", TextVerbose = args.ToString(), Timestamp = DateTime.Now });

                    await DisposeSocketAsync(false).ConfigureAwait(false);
                }).ConfigureAwait(false);
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

                    await DisposeSocketAsync(false).ConfigureAwait(false);
                }).ConfigureAwait(false);
            };

            try
            {
                await socketClient.StartAsync(strategy.Name).ConfigureAwait(true);

                StrategyDisplayViewModel.IsActive = true;

                if (!isForRun)
                {
                    await SetCommandVisibility(StrategyRunnerCommandVisibility.Connected).ConfigureAwait(false);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"MonitorAsync {ex}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"Monitor - {ex.Message}", TextVerbose=ex.ToString(), Timestamp = DateTime.Now });
                await SetCommandVisibility(StrategyRunnerCommandVisibility.ServerUnavailable).ConfigureAwait(false);
                await DisconnectSocketAsync().ConfigureAwait(false);
                return false;
            }
        }

        private async Task OnStrategyNotificationAsync(Socket.Messages.Message message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<TradeView.Core.TradeStrategy.StrategyNotification>>(message.Data);

                foreach (var notification in strategyNotifications)
                {
                    NotificationsAdd(notification.GetMessage());

                    if(notification.NotificationLevel.Equals(TradeView.Core.TradeStrategy.NotificationLevel.DisconnectClient))
                    {
                        await DisconnectSocketAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"OnStrategyNotification {ex}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnStrategyNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private async Task OnTradeNotificationAsync(Socket.Messages.Message message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<TradeView.Core.TradeStrategy.StrategyNotification>>(message.Data);

                var orderedStrategyNotifications = strategyNotifications.OrderBy(n => n.Timestamp).ToList();

                await StrategyDisplayViewModel.TradeNotificationsAsync(orderedStrategyNotifications).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Log($"OnTradeNotification {ex}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnTradeNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private async Task OnCandlesticksNotificationAsync(Socket.Messages.Message message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<TradeView.Core.TradeStrategy.StrategyNotification>>(message.Data);

                var orderedStrategyNotifications = strategyNotifications.OrderBy(n => n.Timestamp).ToList();

                await StrategyDisplayViewModel.CandlestickNotificationsAsync(orderedStrategyNotifications).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                Logger.Log($"OnCandlesticksNotification {ex}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnCandlesticksNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private async Task OnParameterUpdateNotificationAsync(Socket.Messages.Message message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<TradeView.Core.TradeStrategy.StrategyNotification>>(message.Data);

                var latestStrategyNotification = strategyNotifications.OrderBy(n => n.Timestamp).Last();

                Strategy.Parameters = latestStrategyNotification.Message;
            }
            catch (Exception ex)
            {
                Logger.Log($"OnParameterUpdateNotificationAsync {ex}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnParameterUpdateNotificationAsync - {ex.Message}", TextVerbose = ex.ToString() });
            }

            await Task.FromResult<object>(null).ConfigureAwait(false);
        }

        private async Task OnOrderBookNotificationAsync(Socket.Messages.Message message)
        {
            try
            {
                var strategyNotifications = JsonConvert.DeserializeObject<List<TradeView.Core.TradeStrategy.StrategyNotification>>(message.Data);

                var orderedStrategyNotifications = strategyNotifications.OrderBy(n => n.Timestamp).ToList();

                await StrategyDisplayViewModel.OrderNotificationsAsync(orderedStrategyNotifications).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Log($"OnOrderBookNotification {ex}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnOrderBookNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private void OnAccountNotification(Socket.Messages.Message message)
        {
            try
            {
                var accountBalances = AccountViewModel.Account.AccountInfo.Balances.ToList();
            }
            catch (Exception ex)
            {
                Logger.Log($"{message.MethodName} {ex}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"OnAccountNotification - {ex.Message}", TextVerbose = ex.ToString() });
            }
        }

        private void ObserveSymbols()
        {
            var symbolsObservable = Observable.FromEventPattern<StrategySymbolsEventArgs>(
                eventHandler => SymbolsViewModel.OnSymbolsNotification += eventHandler,
                eventHandler => SymbolsViewModel.OnSymbolsNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            symbolsSubscription = symbolsObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = args.Message, TextVerbose = args.Exception.ToString() });
                }
                else if (string.IsNullOrWhiteSpace(args.Message) && args.Value.Any())
                {
                    symbols = args.Value;
                    StrategyDisplayViewModel.Symbols.AddRange(symbols);
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

            accountSubscription = accountObservable.Subscribe(async args =>
            {
                if (args.HasException)
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = args.Message, TextVerbose = args.Exception.ToString() });
                }
                else if(args.AccountEventType.Equals(AccountEventType.LoggedIn))
                {
                    await OrdersViewModel.SetAccount(args.Value).ConfigureAwait(false);
                }
                else if (args.AccountEventType.Equals(AccountEventType.UpdateOrders))
                {
                    await OrdersViewModel.UpdateOrders(args.Value).ConfigureAwait(false);
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

            ordersSubscription = ordersObservable.Subscribe(args =>
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

            parametersSubscription = parametersObservable.Subscribe(async args =>
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

                    await Update(args.Value.Parameters).ConfigureAwait(false);
                }
            });
        }

        private void ObserveStrategy()
        {
            var strategyObservable = Observable.FromEventPattern<StrategyEventArgs>(
                eventHandler => StrategyDisplayViewModel.OnStrategyNotification += eventHandler,
                eventHandler => StrategyDisplayViewModel.OnStrategyNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            strategySubscription = strategyObservable.Subscribe(args =>
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

        private void ObserveServerMonitorCache()
        {
            var serverMonitorCacheObservable = Observable.FromEventPattern<ServerMonitorCacheEventArgs>(
                eventHandler => serverMonitorCache.ServerMonitorCacheNotification += eventHandler,
                eventHandler => serverMonitorCache.ServerMonitorCacheNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            serverMonitorCacheSubscription = serverMonitorCacheObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = args.Message, TextVerbose = args.Exception.ToString() });
                }
                else if (!string.IsNullOrWhiteSpace(args.Message))
                {
                    if(SelectedServer != null
                        && SelectedServer.Name.Equals(args.Message, StringComparison.Ordinal))
                    {
                        ResetCommandVisibility();
                    }
                }
            });
        }

        private void NotificationsAdd(Message message)
        {
            var category = message.MessageType switch
            {
                MessageType.Error => Prism.Logging.Category.Exception,
                MessageType.Warn => Prism.Logging.Category.Warn,
                _ => Prism.Logging.Category.Info,
            };

            Logger.Log(message.Text, category, Prism.Logging.Priority.Low);

            message.Text = $"{message.Timestamp:dd/MM/yyyy hh:mm:ss.fff tt} {message.Text}";
            Notifications.Insert(0, message);
        }

        private void ResetCommandVisibility()
        {
            if (SelectedServer != null
                && SelectedServer.IsConnected)
            {
                SetCommandVisibility(StrategyRunnerCommandVisibility.ServerAvailable).FireAndForget();
                return;
            }

            SetCommandVisibility(StrategyRunnerCommandVisibility.ServerUnavailable).FireAndForget();
        }

        private async Task SetCommandVisibility(StrategyRunnerCommandVisibility strategyRunnerCommandVisibility)
        {
            await commandVisibilitySemaphoreSlim.WaitAsync().ConfigureAwait(false);

            try
            {
                if (strategyRunnerCommandVisibility.Equals(StrategyRunnerCommandVisibility.ServerAvailable)
                    && !IsConnected
                    && !IsConnecting)
                {
                    IsConnecting = true;

                    var isRunning = await IsStrategyRunningAsync().ConfigureAwait(false);

                    CanRun = !isRunning;
                    CanMonitor = isRunning;

                    IsConnecting = false;
                }
                else if (strategyRunnerCommandVisibility.Equals(StrategyRunnerCommandVisibility.Connected)
                    || strategyRunnerCommandVisibility.Equals(StrategyRunnerCommandVisibility.Connecting))
                {
                    CanRun = false;
                    CanMonitor = false;
                    IsConnecting = strategyRunnerCommandVisibility.Equals(StrategyRunnerCommandVisibility.Connecting);
                    IsConnected = strategyRunnerCommandVisibility.Equals(StrategyRunnerCommandVisibility.Connected);
                }
                else if (strategyRunnerCommandVisibility.Equals(StrategyRunnerCommandVisibility.ServerUnavailable))
                {
                    CanRun = false;
                    CanMonitor = false;
                    IsConnecting = false;
                    IsConnected = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"SetCommandVisibility {ex}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);

                NotificationsAdd(new Message { MessageType = MessageType.Error, Text = $"SetCommandVisibility - {ex.Message}", TextVerbose = ex.ToString() });

                await SetCommandVisibility(StrategyRunnerCommandVisibility.ServerUnavailable).ConfigureAwait(false);
            }
            finally
            {
                commandVisibilitySemaphoreSlim.Release();
            }
        }

        private void RaiseStrategyDisplayEvent()
        {
            var onStrategyDisplay = OnStrategyDisplay;
            onStrategyDisplay?.Invoke(this, new StrategyDisplayEventArgs { StrategyAssemblyManager = strategyAssemblyManager });
        }
    }
}
