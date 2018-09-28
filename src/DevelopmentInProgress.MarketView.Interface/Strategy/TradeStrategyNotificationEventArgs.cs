using System;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    public class TradeStrategyNotificationEventArgs : EventArgs
    {
        public StrategyNotification StrategyNotification { get; set; }
    }
}
