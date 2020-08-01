using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using Prism.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel
{
    public class TradeServerViewModel : BaseViewModel
    {
        private TradeServer tradeServer;
        bool disposed = false;

        public TradeServerViewModel(TradeServer tradeServer, ILoggerFacade logger)
            : base(logger)
        {
            this.tradeServer = tradeServer;
        }

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
    }
}
