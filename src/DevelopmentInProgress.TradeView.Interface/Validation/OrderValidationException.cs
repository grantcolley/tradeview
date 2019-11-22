using System;

namespace DevelopmentInProgress.TradeView.Interface.Validation
{
    public class OrderValidationException : Exception
    {
        public OrderValidationException()
        {
        }

        public OrderValidationException(string message)
            : base(message)
        {
        }

        public OrderValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
