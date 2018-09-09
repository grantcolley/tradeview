using DevelopmentInProgress.Wpf.Common.Services;
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public abstract class BaseViewModel : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BaseViewModel(IWpfExchangeService exchangeService)
        {
            ExchangeService = exchangeService;
        }

        public Dispatcher Dispatcher { get; set; }

        public abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected IWpfExchangeService ExchangeService { get; private set; }

        protected void OnPropertyChanged(string propertyName, bool isDirty = false)
        {
            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler != null)
            {
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
