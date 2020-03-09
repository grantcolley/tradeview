using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Controls.Command;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.Model;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.Services;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel
{
    public class ServerMonitorViewModel : DocumentViewModel
    {
        private ObservableCollection<ServerMonitor> servers;
        private ServerMonitor selectedServer;
        private readonly IDashboardService dashboardService;

        public ServerMonitorViewModel(ViewModelContext viewModelContext, IDashboardService dashboardService)
            : base(viewModelContext)
        {
            this.dashboardService = dashboardService;

            SelectItemCommand = new WpfCommand(OnSelectItem);
        }

        public ICommand SelectItemCommand { get; set; }

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
            var serverMonitors = await dashboardService.GetServers();
            Servers = new ObservableCollection<ServerMonitor>(serverMonitors);
        }

        private void OnSelectItem(object param)
        {
            var selectedItem = param as EntityBase;
        }
    }
}