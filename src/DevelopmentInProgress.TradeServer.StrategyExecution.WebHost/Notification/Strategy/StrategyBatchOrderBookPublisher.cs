using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Strategy
{
    public class StrategyBatchOrderBookPublisher : BatchNotification<StrategyNotification>, IBatchNotification<StrategyNotification>
    {
        private readonly IStrategyNotificationPublisher notificationPublisher;

        public StrategyBatchOrderBookPublisher(IStrategyNotificationPublisher notificationPublisher)
        {
            this.notificationPublisher = notificationPublisher;

            Start();
        }

        public override async Task NotifyAsync(IEnumerable<StrategyNotification> notifications, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await notificationPublisher.PublishOrderBookAsync(notifications).ConfigureAwait(false);
        }
    }
}
