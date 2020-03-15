using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Controls.Command;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.Events;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.Model;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.Services;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel
{
    public class ServerMonitorViewModel : DocumentViewModel
    {
        private readonly IDashboardService dashboardService;
        private ObservableCollection<ServerMonitor> servers;
        private List<IDisposable> serverMonitorSubscriptions;
        private ServerMonitor selectedServer;
        private ServerStrategy selectedServerStrategy;
        private bool isLoadingServers;
        private bool disposed;

        public ServerMonitorViewModel(ViewModelContext viewModelContext, IDashboardService dashboardService)
            : base(viewModelContext)
        {
            this.dashboardService = dashboardService;

            IsLoadingServers = true;

            serverMonitorSubscriptions = new List<IDisposable>();
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
                var serverMonitors = await dashboardService.GetServers();

                serverMonitors.ForEach(ObserveServerMonitor);

                Servers = new ObservableCollection<ServerMonitor>(serverMonitors);

                IsLoadingServers = false;

                await Task.WhenAll(serverMonitors.Select(s => s.ConnectAsync(ViewModelContext.UiDispatcher)));
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

        protected async override void OnDisposing()
        {
            if (disposed)
            {
                return;
            }

            foreach(var server in servers)
            {
                await server.DisposeAsync();
            }

            foreach(var serverMonitorSubscription in serverMonitorSubscriptions)
            {
                serverMonitorSubscription.Dispose();
            }

            disposed = true;
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
                    NotificationsAdd(new Message { MessageType = MessageType.Error, Text = args.Message, TextVerbose = args.Exception.ToString() });
                }
                else if (!string.IsNullOrWhiteSpace(args.Message))
                {
                    NotificationsAdd(new Message { MessageType = MessageType.Info, Text = args.Message });
                }
            });

            serverMonitorSubscriptions.Add(serverMonitorSubscription);
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