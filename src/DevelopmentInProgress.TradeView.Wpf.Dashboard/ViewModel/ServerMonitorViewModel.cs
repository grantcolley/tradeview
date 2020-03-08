using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel
{
    public class ServerMonitorViewModel : DocumentViewModel
    {
        private Server server;
        private List<Server> servers;
        private readonly IServerService serverService;

        public ServerMonitorViewModel(ViewModelContext viewModelContext, IServerService serverService)
            : base(viewModelContext)
        {
            this.serverService = serverService;
        }

        public List<Server> Servers
        {
            get { return servers; }
            set
            {
                if(servers != value)
                {
                    servers = value;
                    OnPropertyChanged("Servers");
                }
            }
        }

        public Server Server
        {
            get { return server; }
            set
            {
                if (server != value)
                {
                    server = value;
                    OnPropertyChanged("Server");
                }
            }

        }

        protected async override void OnPublished(object data)
        {
            Servers = await serverService.GetServers();
        }
    }
}