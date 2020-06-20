using DevelopmentInProgress.Socket.Client;
using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class ServerMonitor : EntityBase
    {
        private readonly SemaphoreSlim serverMonitorSemaphoreSlim = new SemaphoreSlim(1, 1);
        private SocketClient socketClient;
        private bool isConnecting;
        private bool isConnected;

        private string name;
        private Uri uri;
        private int maxDegreeOfParallelism;
        private bool enabled;
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

        public Uri Uri
        {
            get { return uri; }
            set
            {
                if (uri != value)
                {
                    uri = value;
                    OnPropertyChanged("Uri");
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

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnPropertyChanged("Enabled");
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

                OnNotification();
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
            finally
            {
                socketClient = null;
                IsConnecting = false;
                IsConnected = false;
            }
        }

        public async Task ConnectAsync(Dispatcher dispatcher)
        {
            await serverMonitorSemaphoreSlim.WaitAsync();

            try
            {
                if(socketClient != null)
                {
                    return;
                }

                var serverIsRunning = await IsServerRunningAsync();

                if(!serverIsRunning)
                {
                    return;
                }

                IsConnecting = true;

                socketClient = new SocketClient($"{Uri}serverhub", Environment.UserName);

                socketClient.On("OnConnected", message =>
                {
                    dispatcher.Invoke(() =>
                    {
                        IsConnecting = false;
                        IsConnected = true;

                        OnNotification();
                    });
                });

                socketClient.On("OnNotification", async (message) =>
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

                    }
                    else
                    {
                        await dispatcher.Invoke(async () =>
                        {
                            OnException(args.Message, new Exception(args.Message));

                            await DisposeSocketAsync();
                        });
                    }
                };

                await socketClient.StartAsync(Name);
            }
            catch(WebSocketException)
            {
                await DisposeSocketAsync();
            }
            catch(Exception ex)
            {
                await DisposeSocketAsync();

                OnException(ex.Message, ex);
            }
            finally
            {
                serverMonitorSemaphoreSlim.Release();
            }
        }

        private async Task<bool> IsServerRunningAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync($"{Uri}ping"))
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return content.Contains("Alive");
                        }
                    }
                }
            }
            catch (Exception)
            {
                // intentionally swallow
            }

            return false;
        }

        private async Task OnServerMonitorNotificationAsync(Socket.Messages.Message message)
        {
            try
            {
                var serverMonitorNotifications = JsonConvert.DeserializeObject<List<Core.Server.ServerNotification>>(message.Data);

                if (serverMonitorNotifications.Any(smn => smn.Equals(Core.Server.ServerNotificationLevel.DisconnectClient)))
                {
                    await DisposeSocketAsync();
                }
                else
                {
                    var serverMonitorNotification = serverMonitorNotifications.OrderByDescending(smn => smn.Timestamp).First();

                    var serverMonitor = JsonConvert.DeserializeObject<Core.Server.ServerMonitor>(serverMonitorNotification.Message);

                    ServerMonitorHelper.UpdateServerMonitor(this, serverMonitor);

                    OnNotification();
                }
            }
            catch (Exception ex)
            {
                OnException(ex.Message, ex);
            }
        }

        private void OnException(string message, Exception exception)
        {
            var onServerMonitorNotification = OnServerMonitorNotification;
            onServerMonitorNotification?.Invoke(this, new ServerMonitorEventArgs { Value = this,  Message = $"{Name} : {message}", Exception = exception });
        }

        private void OnNotification(string message = null)
        {
            var onServerMonitorNotification = OnServerMonitorNotification;
            onServerMonitorNotification?.Invoke(this, new ServerMonitorEventArgs { Value = this, Message = message });
        }
    }
}
