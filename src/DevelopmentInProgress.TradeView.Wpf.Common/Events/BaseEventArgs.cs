using System;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Events
{
    public abstract class BaseEventArgs<T> : EventArgs
    {
        public T Value { get; set; }
        public Exception Exception { get; set; }
        public bool HasException { get { return Exception == null ? false : true; } }
        public string Message { get; set; }
    }
}