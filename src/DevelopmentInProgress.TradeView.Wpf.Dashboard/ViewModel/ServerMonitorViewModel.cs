using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Controls.Command;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.Model;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.Services;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;
using Prism.Logging;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel
{
    public class ServerMonitorViewModel : DocumentViewModel
    {
        private readonly IDashboardService dashboardService;
        private ObservableCollection<ServerMonitor> servers;
        private ServerMonitor selectedServer;
        private bool isLoadingServers;

        public ServerMonitorViewModel(ViewModelContext viewModelContext, IDashboardService dashboardService)
            : base(viewModelContext)
        {
            this.dashboardService = dashboardService;

            SelectItemCommand = new WpfCommand(OnSelectItem);

            IsLoadingServers = true;
        }
        
        public ICommand SelectItemCommand { get; set; }

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
                    OnPropertyChanged("SelectedServer");
                }
            }
        }

        protected async override void OnPublished(object data)
        {
            try
            {
                var serverMonitors = await dashboardService.GetServers();
                Servers = new ObservableCollection<ServerMonitor>(serverMonitors);
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

        private void OnSelectItem(object param)
        {
            var selectedItem = param as EntityBase;
        }
    }
}