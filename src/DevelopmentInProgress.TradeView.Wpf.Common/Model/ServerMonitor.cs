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
    public class ServerMonitor : EntityBase, IDisposable
    {
        private readonly SemaphoreSlim serverMonitorSemaphoreSlim = new SemaphoreSlim(1, 1);
        private SocketClient socketClient;
        private bool isConnecting;
        private bool isConnected;
        private bool disposed;
        private string name;
        private Uri uri;
        private int maxDegreeOfParallelism;
        private bool enabled;
        private string startedBy;
        private string stoppedBy;
        private DateTime started;
        private DateTime stopped;

        public ServerMonitor()
        {
            Strategies = new ObservableCollection<ServerStrategy>();
        }

        public event EventHandler<ServerMonitorEventArgs> OnServerMonitorNotification;

        public ObservableCollection<ServerStrategy> Strategies { get; }

        public string Name 
        {
            get { return name; }
            set
            {
                if(name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
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
                    OnPropertyChanged(nameof(Uri));
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
                    OnPropertyChanged(nameof(MaxDegreeOfParallelism));
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
                    OnPropertyChanged(nameof(Enabled));
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
                    OnPropertyChanged(nameof(StartedBy));
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
                    OnPropertyChanged(nameof(StoppedBy));
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
                    OnPropertyChanged(nameof(Started));
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
                    OnPropertyChanged(nameof(Stopped));
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

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        public int StrategyCount
        {
            get { return Strategies.Count; }
            set { OnPropertyChanged(nameof(StrategyCount)); }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task DisposeAsync()
        {
            await DisposeSocketAsync().ConfigureAwait(false);

            Dispose();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Catch all exceptions and notify observers.")]
        public async Task DisposeSocketAsync()
        {
            try
            {
                if (socketClient != null)
                {
                    await socketClient.DisposeAsync().ConfigureAwait(false);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Catch all exceptions and notify observers.")]
        public async Task ConnectAsync(Dispatcher dispatcher)
        {
            await serverMonitorSemaphoreSlim.WaitAsync().ConfigureAwait(false);

            try
            {
                if(socketClient != null)
                {
                    return;
                }

                if (IsConnected
                    || IsConnecting
                    || string.IsNullOrWhiteSpace(Uri.ToString())
                    || Enabled)
                {
                    return;
                }

                var serverIsRunning = await IsServerRunningAsync().ConfigureAwait(false);

                if(!serverIsRunning)
                {
                    return;
                }

                IsConnecting = true;

                socketClient = new SocketClient(new Uri(Uri, "serverhub"), Environment.UserName);

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
                        await OnServerMonitorNotificationAsync(message).ConfigureAwait(false);
                    }).ConfigureAwait(false);
                });

                socketClient.Closed += async (sender, args) =>
                {
                    await dispatcher.Invoke(async () =>
                    {
                        await DisposeSocketAsync().ConfigureAwait(false);
                    }).ConfigureAwait(false);
                };

                socketClient.Error += async (sender, args) =>
                {
                    var ex = args as Exception;
                    if (ex.InnerException is TaskCanceledException)
                    {
                        await DisposeSocketAsync().ConfigureAwait(false);

                    }
                    else
                    {
                        await dispatcher.Invoke(async () =>
                        {
                            OnException(args.Message, new Exception(args.Message));

                            await DisposeSocketAsync().ConfigureAwait(false);
                        }).ConfigureAwait(false);
                    }
                };

                await socketClient.StartAsync(Name).ConfigureAwait(false);
            }
            catch(WebSocketException)
            {
                await DisposeSocketAsync().ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                await DisposeSocketAsync().ConfigureAwait(false);

                OnException(ex.Message, ex);
            }
            finally
            {
                serverMonitorSemaphoreSlim.Release();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (serverMonitorSemaphoreSlim != null)
                {
                    serverMonitorSemaphoreSlim.Dispose();
                }
            }

            disposed = true;
        }

        private async Task<bool> IsServerRunningAsync()
        {
            try
            {
                using var client = new HttpClient();

                using var response = await client.GetAsync(new Uri(Uri, "ping")).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return content.Contains("Alive", StringComparison.Ordinal);
                }
            }
            catch (HttpRequestException)
            {
                // intentionally swallow
            }

            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Catch all exceptions and notify observers.")]
        private async Task OnServerMonitorNotificationAsync(Socket.Messages.Message message)
        {
            try
            {
                var serverMonitorNotifications = JsonConvert.DeserializeObject<List<Core.Server.ServerNotification>>(message.Data);

                if (serverMonitorNotifications.Any(smn => smn.Equals(Core.Server.ServerNotificationLevel.DisconnectClient)))
                {
                    await DisposeSocketAsync().ConfigureAwait(false);
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
