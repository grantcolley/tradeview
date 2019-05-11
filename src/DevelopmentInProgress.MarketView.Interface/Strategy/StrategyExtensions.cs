using System;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    public static class StrategyExtensions
    {
        public static StrategyNotification GetNotification(this Strategy strategy, NotificationLevel notificationLevel, int notificationEvent, string message)
        {
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