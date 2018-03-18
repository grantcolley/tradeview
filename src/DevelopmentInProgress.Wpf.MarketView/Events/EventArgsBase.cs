using System;

namespace DevelopmentInProgress.Wpf.MarketView.Events
{
    public abstract class EventArgsBase<T> : EventArgs
    {
        public T Value { get; set; }
        public Exception Exception { get; set; }
        public bool HasException { get { return Exception == null ? false : true; } }
    }
}