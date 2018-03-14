using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class TradeViewModel : BaseViewModel
    {
        private bool disposed;
        private bool isLoading;

        public TradeViewModel(Account account, IExchangeService exchangeService, Action<Exception> exception)
            : base(exchangeService)
        {
            IsLoading = true;
        }

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged("IsLoading");
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
                // do tuff here...
            }

            disposed = true;
        }
    }
}
