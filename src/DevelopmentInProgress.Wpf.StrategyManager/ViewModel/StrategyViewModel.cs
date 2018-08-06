using DevelopmentInProgress.Wpf.Controls.Messaging;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.StrategyManager.Model;
using DevelopmentInProgress.Wpf.StrategyManager.Services;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DevelopmentInProgress.Wpf.StrategyManager.ViewModel
{
    public class StrategyViewModel : DocumentViewModel
    {
        private IStrategyService strategyService;
        private Strategy strategy;
        private bool isRunEnabled;
        private bool isMonitoEnabled;
        private bool isConnected;
        private bool isStopEnabled;
        private HubConnection hubConnection;
        private ObservableCollection<Message> notifications;

        public StrategyViewModel(ViewModelContext viewModelContext, IStrategyService strategyService)
            : base(viewModelContext)
        {
            this.strategyService = strategyService;

            IsRunEnabled = true;
            IsMonitoEnabled = true;
            //IsConnected = true;
            //IsStopEnabled = true;

            Notifications = new ObservableCollection<Message>();

            RunCommand = new ViewModelCommand(RunStrategy);
            MonitorCommand = new ViewModelCommand(MonitorStrategy);
            DisconnectCommand = new ViewModelCommand(Disconnect);
            StopCommand = new ViewModelCommand(StopStrategy);
        }

        public ICommand RunCommand { get; set; }
        public ICommand MonitorCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand StopCommand { get; set; }

        public HubConnection HubConnection
        {
            get { return hubConnection; }
            set { hubConnection = value; }
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

        public bool IsRunEnabled
        {
            get { return isRunEnabled; }
            set
            {
                if (isRunEnabled != value)
                {
                    isRunEnabled = value;
                    OnPropertyChanged("IsRunEnabled");
                }
            }
        }

        public bool IsMonitoEnabled
        {
            get { return isMonitoEnabled; }
            set
            {
                if (isMonitoEnabled != value)
                {
                    isMonitoEnabled = value;
                    OnPropertyChanged("IsMonitoEnabled");
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

        public bool IsStopEnabled
        {
            get { return isStopEnabled; }
            set
            {
                if (isStopEnabled != value)
                {
                    isStopEnabled = value;
                    OnPropertyChanged("IsStopEnabled");
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

        private void Disconnect(object param)
        {

        }

        private void StopStrategy(object param)
        {

        }

        protected async override void OnDisposing()
        {
            await Disconnect();
        }

        private async Task Disconnect()
        {
            if (HubConnection != null)
            {
                await HubConnection.DisposeAsync();
                HubConnection = null;
                IsConnected = false;
            }
        }

        private async Task Run()
        {
            try
            {
                var result = await Monitor();

                if (result)
                {
                    var jsonContent = JsonConvert.SerializeObject(Strategy);
                    var dependencies = strategy.Dependencies.Select(d => d.File);

                    var strategyRunnerClient = new DevelopmentInProgress.MarketView.Interface.TradeStrategy.StrategyRunnerClient();

                    var response = await strategyRunnerClient.PostAsync($"{Strategy.StrategyServerUrl}/notificationhub", jsonContent, dependencies);
                }
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
            }
        }
        
        private async Task<bool> Monitor()
        {
            if(IsConnected)
            {
                return IsConnected;
            }

            HubConnection = new HubConnectionBuilder()
                .WithUrl($"{Strategy.StrategyServerUrl}/runstrategy?strategyname={Strategy.Name}", HttpTransportType.WebSockets)
                .Build();

            HubConnection.On<object>("Connected", message =>
            {
                ViewModelContext.UiDispatcher.Invoke(() =>
                {
                    OnConnected(new Message { MessageType = MessageType.Info, Text = $"{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff tt")} {message.ToString()}", Timestamp = DateTime.Now });
                });
            });

            HubConnection.On<object>("Send", (message) =>
            {
                ViewModelContext.UiDispatcher.Invoke(() =>
                {
                    OnNotificationRecieved(message);
                });
            });

            try
            {
                await HubConnection.StartAsync();

                IsConnected = true;

                return IsConnected;
            }
            catch (Exception ex)
            {
                OnConnected(new Message { MessageType = MessageType.Error, Text = $"{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss.fff tt")} Failed to connect", TextVerbose=ex.ToString(), Timestamp = DateTime.Now });
                throw;
            }
        }

        private void OnConnected(Message message)
        {
            Notifications.Add(message);
        }

        private void OnNotificationRecieved(object message)
        {

        }
    }
}
