using System;
using DevelopmentInProgress.TradeView.Core.Strategy;

namespace DevelopmentInProgress.TradeView.Core.Extensions
{
    public static class StrategyExtensions
    {
        public static StrategyNotification GetNotification(this Strategy.Strategy strategy, NotificationLevel notificationLevel, int notificationEvent, string message)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            return new StrategyNotification
            {
                Id = strategy.Id,
                Name = strategy.Name,
                Status = strategy.Status,
                StrategySubscriptions = strategy.StrategySubscriptions,
                TargetAssembly = strategy.TargetAssembly,
                TargetType = strategy.TargetType,
                Parameters = strategy.Parameters,
                Machine = Environment.MachineName,
                NotificationEvent = notificationEvent,
                NotificationLevel = notificationLevel,
                Message = message
            };
        }
    }
}