using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Strategy
{
    public interface IStrategyNotificationPublisher
    {
        Task PublishCustomNotificationsAsync(string methodName, IEnumerable<StrategyNotification> notifications);
        Task PublishNotificationsAsync(IEnumerable<StrategyNotification> notifications);
        Task PublishTradesAsync(IEnumerable<StrategyNotification> notifications);
        Task PublishOrderBookAsync(IEnumerable<StrategyNotification> notifications);
        Task PublishCandlesticksAsync(IEnumerable<StrategyNotification> notifications);
        Task PublishStatisticsAsync(IEnumerable<StrategyNotification> notifications);
        Task PublishAccountInfoAsync(IEnumerable<StrategyNotification> notifications);
        Task PublishParameterUpdateAsync(IEnumerable<StrategyNotification> notifications);
    }
}
