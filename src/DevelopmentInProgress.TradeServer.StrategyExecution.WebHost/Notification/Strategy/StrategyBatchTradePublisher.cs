using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Strategy
{
    public class StrategyBatchTradePublisher : BatchNotification<StrategyNotification>, IBatchNotification<StrategyNotification>
    {
        private readonly IStrategyNotificationPublisher notificationPublisher;

        public StrategyBatchTradePublisher(IStrategyNotificationPublisher notificationPublisher)
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

            await notificationPublisher.PublishTradesAsync(notifications).ConfigureAwait(false);
        }
    }
}
