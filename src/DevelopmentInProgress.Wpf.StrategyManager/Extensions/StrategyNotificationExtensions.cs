using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.Wpf.Controls.Messaging;

namespace DevelopmentInProgress.Wpf.StrategyManager.Extensions
{
    public static class StrategyNotificationExtensions
    {
        public static Message GetMessage(this StrategyNotification strategyNotification)
        {
            MessageType messageType;
            switch(strategyNotification.NotificationLevel)
            {
                case NotificationLevel.Error:
                    messageType = MessageType.Error;
                    break;
                case NotificationLevel.Warning:
                    messageType = MessageType.Warn;
                    break;
                default:
                    messageType = MessageType.Info;
                    break;
            }

            return new Message
            {
                MessageType = messageType,
                Text = strategyNotification.Message,
                Timestamp = strategyNotification.Timestamp,
                TextVerbose = strategyNotification.ToString()
            };
        }
    }
}
