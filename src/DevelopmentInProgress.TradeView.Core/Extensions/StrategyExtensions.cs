using System;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;

namespace DevelopmentInProgress.TradeView.Core.Extensions
{
    public static class StrategyExtensions
    {
        public static StrategyNotification GetNotification(this TradeStrategy.Strategy strategy, NotificationLevel notificationLevel, int notificationEvent, string message)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            var strategyNotification = new StrategyNotification
            {
                Id = strategy.Id,
                Name = strategy.Name,
                Status = strategy.Status,
                TargetAssembly = strategy.TargetAssembly,
                TargetType = strategy.TargetType,
                Parameters = strategy.Parameters,
                Machine = Environment.MachineName,
                NotificationEvent = notificationEvent,
                NotificationLevel = notificationLevel,
                Message = message
            };

            strategyNotification.StrategySubscriptions.AddRange(strategy.StrategySubscriptions);

            return strategyNotification;
        }
    }
}