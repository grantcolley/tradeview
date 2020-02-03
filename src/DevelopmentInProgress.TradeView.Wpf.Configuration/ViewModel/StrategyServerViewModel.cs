using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using Prism.Logging;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel
{
    public class StrategyServerViewModel : BaseViewModel
    {
        private IStrategyServerService strategyServerService;
        private StrategyServer strategyServer;
        bool disposed = false;

        public StrategyServerViewModel(StrategyServer strategyServer, IStrategyServerService strategyServerService, ILoggerFacade logger)
            : base(logger)
        {
            this.strategyServer = strategyServer;
            this.strategyServerService = strategyServerService;
        }

        public event EventHandler<StrategyServerEventArgs> OnStrategyServerNotification;

        public StrategyServer StrategyServer
        {
            get { return strategyServer; }
            set
            {
                if (strategyServer != value)
                {
                    strategyServer = value;
                    OnPropertyChanged("StrategyServer");
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
            var onStrategyServerNotification = OnStrategyServerNotification;
            onStrategyServerNotification?.Invoke(this, new StrategyServerEventArgs { Value = StrategyServer, Exception = exception });
        }
    }
}
