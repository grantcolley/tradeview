using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Extensions;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Strategy
{
    public class StrategyBatchCustomNotificationPublisher : BatchNotification<StrategyNotification>, IBatchNotification<StrategyNotification>
    {
        private readonly IStrategyNotificationPublisher notificationPublisher;

        public StrategyBatchCustomNotificationPublisher(IStrategyNotificationPublisher notificationPublisher)
        {
            this.notificationPublisher = notificationPublisher;

            Start();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        public override Task NotifyAsync(IEnumerable<StrategyNotification> notifications, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    var methodNameGroups = notifications.GroupBy(n => n.MethodName, n => n).ToList();
                    foreach (var methodNameGroup in methodNameGroups)
                    {
                        notificationPublisher.PublishCustomNotificationsAsync(methodNameGroup.Key, notifications).FireAndForget();
                    }
                }

                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }
    }
}