using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using Microsoft.Extensions.Logging;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Strategy
{
    public class StrategyBatchNotificationFactory : BatchNotificationFactory<StrategyNotification>
    {
        private readonly IStrategyNotificationPublisher notificationPublisher;
        private readonly ILoggerFactory loggerFactory;

        public StrategyBatchNotificationFactory(IStrategyNotificationPublisher notificationPublisher, ILoggerFactory loggerFactory)
        {
            this.notificationPublisher = notificationPublisher;
            this.loggerFactory = loggerFactory;
        }

        public override IBatchNotification<StrategyNotification> GetBatchNotifier(BatchNotificationType batchNotifierType)
        {
            return batchNotifierType switch
            {
                BatchNotificationType.StrategyLogger => new StrategyBatchLogger(loggerFactory),
                BatchNotificationType.StrategyAccountInfoPublisher => new StrategyBatchAccountInfoPublisher(notificationPublisher),
                BatchNotificationType.StrategyCustomNotificationPublisher => new StrategyBatchCustomNotificationPublisher(notificationPublisher),
                BatchNotificationType.StrategyNotificationPublisher => new StrategyBatchNotificationPublisher(notificationPublisher),
                BatchNotificationType.StrategyOrderBookPublisher => new StrategyBatchOrderBookPublisher(notificationPublisher),
                BatchNotificationType.StrategyTradePublisher => new StrategyBatchTradePublisher(notificationPublisher),
                BatchNotificationType.StrategyCandlesticksPublisher => new StrategyBatchCandlesticksPublisher(notificationPublisher),
                BatchNotificationType.StrategyStatisticsPublisher => new StrategyBatchStatisticsPublisher(notificationPublisher),
                BatchNotificationType.StrategyParameterUpdatePublisher => new StrategyBatchParameterUpdatePublisher(notificationPublisher),
                _ => null,
            };
        }
    }
}
