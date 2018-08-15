using DevelopmentInProgress.Wpf.Controls.Messaging;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.StrategyManager.Extensions;
using DevelopmentInProgress.Wpf.StrategyManager.Model;
using DevelopmentInProgress.Wpf.StrategyManager.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
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
                await hubConnection.DisposeAsync();
                hubConnection = null;
                IsConnected = false;
            }
        }

        private async Task Run()
        {
            try
            {
                var result = await Monitor(true).ConfigureAwait(false);

                if (result)
                {
                    SetCommandVisibility(false, false, true);

                    var jsonContent = JsonConvert.SerializeObject(Strategy.GetInterfaceStrategy());
                    var dependencies = strategy.Dependencies.Select(d => d.File);

                    var strategyRunnerClient = new MarketView.Interface.TradeStrategy.StrategyRunnerClient();

                    var response = await strategyRunnerClient.PostAsync($"{Strategy.StrategyServerUrl}/runstrategy", jsonContent, dependencies).ConfigureAwait(false);

                    if(response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        await Disconnect().ConfigureAwait(false);
                    }

                    var content = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                    ViewModelContext.UiDispatcher.Invoke(() =>
                    {
                        OnConnected(
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
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
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

            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{Strategy.StrategyServerUrl}/notificationhub?strategyname={Strategy.Name}")
                .Build();

            hubConnection.On<object>("Connected", message =>
            {
                ViewModelContext.UiDispatcher.Invoke(() =>
                {
                    OnConnected(new Message { MessageType = MessageType.Info, Text = $"{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff tt")} {message.ToString()}", Timestamp = DateTime.Now });
                });
            });

            hubConnection.On<object>("Send", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(() =>
                {
                    OnNotificationRecieved(message);
                });
            });

            try
            {
                await hubConnection.StartAsync().ConfigureAwait(false);

                if (!isForRun)
                {
                    SetCommandVisibility(false, false, true);
                }

                return true;
            }
            catch (Exception ex)
            {
                OnConnected(new Message { MessageType = MessageType.Error, Text = $"{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff tt")} Failed to connect", TextVerbose=ex.ToString(), Timestamp = DateTime.Now });
                SetCommandVisibility(true, false, false);
                await Disconnect().ConfigureAwait(false);
                return false;
            }
        }

        private void OnConnected(Message message)
        {
            Notifications.Add(message);
        }

        private void OnNotificationRecieved(object message)
        {

        }

        private void SetCommandVisibility(bool canconnect, bool connecting, bool connected)
        {
            CanConnect = canconnect;
            IsConnecting = connecting;
            IsConnected = connected;
        }
    }
}
