using Prism.Logging;
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace DevelopmentInProgress.TradeView.Wpf.Common.ViewModel
{
    public abstract class BaseViewModel : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BaseViewModel(ILoggerFacade logger)
        {
            Logger = logger;
        }

        public ILoggerFacade Logger { get; private set; }

        public virtual Dispatcher Dispatcher { get; set; }

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var propertyChangedHandler = PropertyChanged;
            propertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
