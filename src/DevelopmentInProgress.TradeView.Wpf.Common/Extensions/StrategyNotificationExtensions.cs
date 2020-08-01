using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class StrategyNotificationExtensions
    {
        public static Message GetMessage(this StrategyNotification strategyNotification)
        {
            if (strategyNotification == null)
            {
                throw new ArgumentNullException(nameof(strategyNotification));
            }

            MessageType messageType = strategyNotification.NotificationLevel switch
            {
                NotificationLevel.Error => MessageType.Error,
                NotificationLevel.Warning => MessageType.Warn,
                _ => MessageType.Info
            };

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
