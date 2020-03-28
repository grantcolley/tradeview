using DevelopmentInProgress.TradeView.Data;
using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Cache
{
    public sealed class ServerMonitorCache : IServerMonitorCache
    {
        private readonly ITradeViewConfigurationServer configurationServer;
        private readonly ObservableCollection<ServerMonitor> serverMonitors;
        private readonly SemaphoreSlim serverMonitorSemaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly Dictionary<string, IDisposable> serverMonitorSubscriptions;
        private IDisposable observableInterval;
        private Dispatcher dispatcher;
        private bool disposed;

        public ServerMonitorCache(ITradeViewConfigurationServer configurationServer)
        {
            this.configurationServer = configurationServer;
            serverMonitors = new ObservableCollection<ServerMonitor>();
            serverMonitorSubscriptions = new Dictionary<string, IDisposable>();

            dispatcher = Application.Current.Dispatcher;
        }

        public event EventHandler<ServerMonitorCacheEventArgs> ServerMonitorCacheNotification;

        public async Task<ObservableCollection<ServerMonitor>> GetServerMonitorsAsync()
        {
            await RefreshServerMonitorsAsync();
            return serverMonitors;
        }

        public async Task RefreshServerMonitorsAsync()
        {
            await serverMonitorSemaphoreSlim.WaitAsync();

            try
            {
                var servers = await configurationServer.GetServersAsync();

                await dispatcher.InvokeAsync(async () =>
                {
                    var removeServers = serverMonitors.Where(sm => !servers.Any(s => s.Name == sm.Name)).ToList();

                    if (removeServers.Any())
                    {
                        await Task.WhenAll(removeServers.Select(s => s.DisposeAsync()).ToList());

                        foreach (var server in removeServers)
                        {
                            serverMonitorSubscriptions[server.Name].Dispose();
                            serverMonitorSubscriptions.Remove(server.Name);
                            serverMonitors.Remove(server);
                        }
                    }

                    var newServers = servers.Where(s => !serverMonitors.Any(sm => sm.Name == s.Name && !string.IsNullOrWhiteSpace(s.Url))).ToList();

                    var newServerMonitors = newServers.Select(s => s.ToServerMonitor()).ToList();

                    foreach (var newServerMonitor in newServerMonitors)
                    {
                        serverMonitors.Add(newServerMonitor);
                        ObserveServerMonitor(newServerMonitor);
                    }
                });

                StartObserveringServers();
            }
            catch (Exception ex)
            {
                OnServerMonitorCacheNotification($"Refreshing Servers : {ex.Message}", ex);
            }
            finally
            {
                serverMonitorSemaphoreSlim.Release();
            }
        }

        public async void Dispose()
        {
            if (disposed)
            {
                return;
            }

            if (observableInterval != null)
            {
                observableInterval.Dispose();
            }

            foreach(var serverMonitorSubscription in serverMonitorSubscriptions.Values)
            {
                serverMonitorSubscription.Dispose();
            }

            await Task.WhenAll(serverMonitors.Select(s => s.DisposeAsync()).ToList());

            disposed = true;
        }

        private void StartObserveringServers()
        {
            if(observableInterval != null)
            {
                return;
            }

            observableInterval = Observable.Interval(TimeSpan.FromSeconds(5))
                .Subscribe(async i =>
                {
                    try
                    {
                        await TryConnectServersAsync(serverMonitors.ToList());
                    }
                    catch (Exception ex)
                    {
                        OnServerMonitorCacheNotification($"Observing Servers : {ex.Message}", ex);
                    }
                });
        }

        private async Task TryConnectServersAsync(IEnumerable<ServerMonitor> servers)
        {
            await Task.WhenAll(servers
                .Where(s => !s.IsConnected && !string.IsNullOrWhiteSpace(s.Url))
                .Select(s => s.ConnectAsync(dispatcher)).ToList());
        }

        private void OnServerMonitorCacheNotification(string message, Exception exception = null)
        {
            var serverMonitorCacheNotification = ServerMonitorCacheNotification;
            serverMonitorCacheNotification?.Invoke(this, new ServerMonitorCacheEventArgs { Value = this, Message = message, Exception = exception });
        }

        private void ObserveServerMonitor(ServerMonitor serverMonitor)
        {
            var serverMonitorObservable = Observable.FromEventPattern<ServerMonitorEventArgs>(
                eventHandler => serverMonitor.OnServerMonitorNotification += eventHandler,
                eventHandler => serverMonitor.OnServerMonitorNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            var serverMonitorSubscription = serverMonitorObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    OnServerMonitorCacheNotification($"{args.Value.Name} : {args.Exception.Message}", args.Exception);
                }
            });

            serverMonitorSubscriptions.Add(serverMonitor.Name, serverMonitorSubscription);
        }
    }
}
