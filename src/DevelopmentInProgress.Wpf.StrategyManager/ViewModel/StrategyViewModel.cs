using DevelopmentInProgress.Wpf.Controls.Messaging;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.StrategyManager.Extensions;
using DevelopmentInProgress.Wpf.StrategyManager.Model;
using DevelopmentInProgress.Wpf.StrategyManager.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public StrategyViewModel(ViewModelContext viewModelContext, IStrategyService strategyService)
            : base(viewModelContext)
        {
            this.strategyService = strategyService;

            CanConnect = true;
            IsConnected = false;
            IsConnecting = false;

            Notifications = new ObservableCollection<Message>();

            RunCommand = new ViewModelCommand(RunStrategy);
            MonitorCommand = new ViewModelCommand(MonitorStrategy);
            DisconnectCommand = new ViewModelCommand(Disconnect);
            StopCommand = new ViewModelCommand(StopStrategy);
            ClearNotificationsCommand = new ViewModelCommand(ClearNotifications);
        }

        public ICommand RunCommand { get; set; }
        public ICommand MonitorCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand ClearNotificationsCommand { get; set; }

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

        private async void RunStrategy(object param)
        {
            await Run();
        }

        private async void MonitorStrategy(object param)
        {
            await Monitor();
        }

        private async void Disconnect(object param)
        {
            await Disconnect();
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
            await Disconnect();
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
                    NotificationsAdd(notification.GetMessage());
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
