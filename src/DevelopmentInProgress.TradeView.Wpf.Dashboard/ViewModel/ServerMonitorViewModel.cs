using DevelopmentInProgress.TradeView.Wpf.Common.Cache;
using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel;
using Prism.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel
{
    public class ServerMonitorViewModel : DocumentViewModel
    {
        private readonly IServerMonitorCache serverMonitorCache;
        private IDisposable serverMonitorCacheSubscription;
        private ObservableCollection<ServerMonitor> servers;
        private ServerMonitor selectedServer;
        private ServerStrategy selectedServerStrategy;
        private bool isLoadingServers;
        private bool disposed;

        public ServerMonitorViewModel(ViewModelContext viewModelContext, IServerMonitorCache serverMonitorCache)
            : base(viewModelContext)
        {
            this.serverMonitorCache = serverMonitorCache;
            IsLoadingServers = true;

            ObserveServerMonitorCache();
        }

        public bool IsLoadingServers
        {
            get { return isLoadingServers; }
            set
            {
                if(isLoadingServers != value)
                {
                    isLoadingServers = value;
                    OnPropertyChanged(nameof(IsLoadingServers));
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Setter required as it points to the server monitor cache.")]
        public ObservableCollection<ServerMonitor> Servers
        {
            get { return servers; }
            set
            {
                if (servers != value)
                {
                    servers = value;

                    if(servers.Any())
                    {
                        SelectedServer = servers.First();
                    }

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
                    if(selectedServer != null
                        && selectedServer.Strategies.Any())
                    {
                        SelectedServerStrategy = selectedServer.Strategies.First();
                    }

                    OnPropertyChanged(nameof(SelectedServer));
                }
            }
        }

        public ServerStrategy SelectedServerStrategy
        {
            get { return selectedServerStrategy; }
            set
            {
                if(selectedServerStrategy != value)
                {
                    selectedServerStrategy = value;
                    OnPropertyChanged(nameof(SelectedServerStrategy));
                }
            }
        }

        protected async override void OnPublished(object data)
        {
            try
            {
                Servers = await serverMonitorCache.GetServerMonitorsAsync();
            }
            catch(Exception ex)
            {
                Logger.Log(ex.ToString(), Category.Exception, Priority.High);
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message, TextVerbose = ex.StackTrace });
            }
            finally
            {
                IsLoadingServers = false;
            }
        }

        protected override void OnDisposing()
        {
            if (disposed)
            {
                return;
            }

            serverMonitorCacheSubscription.Dispose();

            disposed = true;
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
                    NotificationsAdd(new Message { MessageType = MessageType.Info, Text = args.Message });
                }
            });
        }

        private void NotificationsAdd(Message message)
        {
            var category = message.MessageType switch
            {
                MessageType.Error => Category.Exception,
                MessageType.Warn => Category.Warn,
                _ => Category.Info,
            };

            Logger.Log(message.Text, category, Priority.Low);

            message.Text = $"{message.Timestamp:dd/MM/yyyy hh:mm:ss.fff tt} {message.Text}";
            ShowMessage(message);
        }
    }
}