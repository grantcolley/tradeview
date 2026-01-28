using DevelopmentInProgress.TradeView.Wpf.Controls.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace DevelopmentInProgress.TradeView.Wpf.Common.ViewModel
{
    public abstract class BaseViewModel : LoggingBase, IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BaseViewModel(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

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
