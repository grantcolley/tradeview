using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.Events;
using DipSocket.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.Model
{
    public class ServerMonitor : EntityBase
    {
        private DipSocketClient socketClient;
        private bool isConnecting;
        private bool isConnected;

        private string name;
        private string url;
        private int maxDegreeOfParallelism;
        private string startedBy;
        private string stoppedBy;
        private DateTime started;
        private DateTime stopped;
        private ObservableCollection<ServerStrategy> strategies;

        public ServerMonitor()
        {
            Strategies = new ObservableCollection<ServerStrategy>();
        }

        public event EventHandler<ServerMonitorEventArgs> OnServerMonitorNotification;

        public string Name 
        {
            get { return name; }
            set
            {
                if(name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Url
        {
            get { return url; }
            set
            {
                if (url != value)
                {
                    url = value;
                    OnPropertyChanged("Url");
                }
            }
        }

        public int MaxDegreeOfParallelism
        {
            get { return maxDegreeOfParallelism; }
            set
            {
                if (maxDegreeOfParallelism != value)
                {
                    maxDegreeOfParallelism = value;
                    OnPropertyChanged("MaxDegreeOfParallelism");
                }
            }
        }

        public string StartedBy
        {
            get { return startedBy; }
            set
            {
                if (startedBy != value)
                {
                    startedBy = value;
                    OnPropertyChanged("StartedBy");
                }
            }
        }

        public string StoppedBy
        {
            get { return stoppedBy; }
            set
            {
                if (stoppedBy != value)
                {
                    stoppedBy = value;
                    OnPropertyChanged("StoppedBy");
                }
            }
        }

        public DateTime Started
        {
            get { return started; }
            set
            {
                if (started != value)
                {
                    started = value;
                    OnPropertyChanged("Started");
                }
            }
        }

        public DateTime Stopped
        {
            get { return stopped; }
            set
            {
                if (stopped != value)
                {
                    stopped = value;
                    OnPropertyChanged("Stopped");
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

        public ObservableCollection<ServerStrategy> Strategies
        {
            get { return strategies; }
            set
            {
                if (strategies != value)
                {
                    strategies = value;
                    OnPropertyChanged("Strategies");
                    OnPropertyChanged("StrategyCount");
                }
            }
        }

        public int StrategyCount
        {
            get { return strategies.Count; }
            set { OnPropertyChanged("StrategyCount"); }
        }

        public Task DisposeAsync()
        {
            return DisposeSocketAsync();
        }

        public async Task DisposeSocketAsync()
        {
            try
            {
                if (socketClient != null)
                {
                    await socketClient.DisposeAsync();
                }

                IsConnecting = false;
                IsConnected = false;
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
            finally
            {
                socketClient = null;
            }
        }

        public async Task ConnectAsync(Dispatcher dispatcher)
        {
            IsConnecting = true;

            try
            {
                socketClient = new DipSocketClient($"{Url}/serverhub", Environment.UserName);

                socketClient.On("OnConnected", message =>
                {
                    dispatcher.Invoke(() =>
                    {
                        IsConnecting = false;
                        IsConnected = true;
                    });
                });

                socketClient.On("Notification", async (message) =>
                {
                    await dispatcher.Invoke(async () =>
                    {
                        await OnServerMonitorNotificationAsync(message);
                    });
                });

                socketClient.Closed += async (sender, args) =>
                {
                    await dispatcher.Invoke(async () =>
                    {
                        await DisposeSocketAsync();
                    });
                };

                socketClient.Error += async (sender, args) =>
                {
                    var ex = args as Exception;
                    if (ex.InnerException is TaskCanceledException)
                    {
                        await DisposeSocketAsync();
                        return;
                    }

                    await dispatcher.Invoke(async () =>
                    {
                        OnException(args.Message, new Exception(args.Message));

                        await DisposeSocketAsync();
                    });
                };

                await socketClient.StartAsync(Name);
            }
            catch(Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }

        private async Task OnServerMonitorNotificationAsync(DipSocket.Messages.Message message)
        {
            try
            {
                var serverMonitorNotifications = JsonConvert.DeserializeObject<List<Interface.Server.ServerNotification>>(message.Data);

                if (serverMonitorNotifications.Any(smn => smn.Equals(Interface.Server.ServerNotificationLevel.DisconnectClient)))
                {
                    await DisposeSocketAsync();

                    return;
                }

                var serverMonitorNotification = serverMonitorNotifications.OrderByDescending(smn => smn.Timestamp).First();

                // do update here...

            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }

        private void OnException(string message, Exception exception)
        {
            var onServerMonitorNotification = OnServerMonitorNotification;
            onServerMonitorNotification?.Invoke(this, new ServerMonitorEventArgs { Message = message, Exception = exception });
        }

        private void ServerNotification(ServerMonitor serverMonitor)
        {
            var onServerMonitorNotification = OnServerMonitorNotification;
            onServerMonitorNotification?.Invoke(this, new ServerMonitorEventArgs { Value = serverMonitor });
        }
    }
}
