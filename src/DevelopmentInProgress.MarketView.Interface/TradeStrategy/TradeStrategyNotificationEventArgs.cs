using System;

namespace DevelopmentInProgress.MarketView.Interface.TradeStrategy
{
    public class TradeStrategyNotificationEventArgs : EventArgs
    {
        public StrategyNotification StrategyNotification { get; set; }
    }
}
