using DevelopmentInProgress.TradeView.Wpf.Common.Cache;
using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;
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
        }

        public bool IsLoadingServers
        {
            get { return isLoadingServers; }
            set
            {
                if(isLoadingServers != value)
                {
                    isLoadingServers = value;
                    OnPropertyChanged("IsLoadingServers");
                }
            }
        }

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

                    OnPropertyChanged("Servers");
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

                    OnPropertyChanged("SelectedServer");
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
                    OnPropertyChanged("SelectedServerStrategy");
                }
            }
        }

        protected async override void OnPublished(object data)
        {
            try
            {
                ObserveServerMonitorCache();

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

            if(serverMonitorCacheSubscription != null)
            {
                serverMonitorCacheSubscription.Dispose();
            }

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
            Category category;

            switch (message.MessageType)
            {
                case MessageType.Error:
                    category = Category.Exception;
                    break;
                case MessageType.Warn:
                    category = Category.Warn;
                    break;
                default:
                    category = Category.Info;
                    break;
            }

            Logger.Log(message.Text, category, Priority.Low);

            message.Text = $"{message.Timestamp.ToString("dd/MM/yyyy hh:mm:ss.fff tt")} {message.Text}";
            ShowMessage(message);
        }
    }
}