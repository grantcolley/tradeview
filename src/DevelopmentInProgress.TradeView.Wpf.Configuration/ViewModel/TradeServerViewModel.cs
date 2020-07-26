using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using Prism.Logging;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel
{
    public class TradeServerViewModel : BaseViewModel
    {
        private readonly ITradeServerService tradeServerService;
        private TradeServer tradeServer;
        bool disposed = false;

        public TradeServerViewModel(TradeServer tradeServer, ITradeServerService tradeServerService, ILoggerFacade logger)
            : base(logger)
        {
            this.tradeServer = tradeServer;
            this.tradeServerService = tradeServerService;
        }

        public event EventHandler<TradeServerEventArgs> OnTradeServerNotification;

        public TradeServer TradeServer
        {
            get { return tradeServer; }
            set
            {
                if (tradeServer != value)
                {
                    tradeServer = value;
                    OnPropertyChanged(nameof(TradeServer));
                }
            }
        }

        protected override void Dispose(bool disposing)
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

        private void OnTradeServerException(Exception exception)
        {
            var onServerNotification = OnTradeServerNotification;
            onServerNotification?.Invoke(this, new TradeServerEventArgs { Value = TradeServer, Exception = exception });
        }
    }
}
