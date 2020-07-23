using System;

namespace DevelopmentInProgress.TradeView.Core.Exceptions
{
    public class CancelOrderException : Exception
    {
        public CancelOrderException()
        {
        }

        public CancelOrderException(string message) : base(message)
        {
        }

        public CancelOrderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
