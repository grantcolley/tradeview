using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class TradeViewModel : BaseViewModel
    {
        private bool disposed;

        public TradeViewModel(Account account, IExchangeService exchangeService, Action<Exception> exception)
            : base(exchangeService)
        {

        }

        public override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // do tuff here...
            }

            disposed = true;
        }
    }
}
