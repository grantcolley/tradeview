using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using Prism.Logging;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel
{
    public class ServerViewModel : BaseViewModel
    {
        private IServerService serverService;
        private Server server;
        bool disposed = false;

        public ServerViewModel(Server server, IServerService serverService, ILoggerFacade logger)
            : base(logger)
        {
            this.server = server;
            this.serverService = serverService;
        }

        public event EventHandler<ServerEventArgs> OnServerNotification;

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

        public override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // dispose stuff...
            }

            disposed = true;
        }

        private void OnStrategyException(Exception exception)
        {
            var onServerNotification = OnServerNotification;
            onServerNotification?.Invoke(this, new ServerEventArgs { Value = Server, Exception = exception });
        }
    }
}
