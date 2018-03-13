using DevelopmentInProgress.Wpf.MarketView.Services;
using System;
using System.ComponentModel;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public abstract class BaseViewModel : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BaseViewModel(IExchangeService exchangeService)
        {
            ExchangeService = exchangeService;
        }

        public abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected IExchangeService ExchangeService { get; private set; }

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
