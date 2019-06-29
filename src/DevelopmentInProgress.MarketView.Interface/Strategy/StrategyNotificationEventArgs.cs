using System;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    public class StrategyNotificationEventArgs : EventArgs
    {
        public StrategyNotification StrategyNotification { get; set; }
    }
}
