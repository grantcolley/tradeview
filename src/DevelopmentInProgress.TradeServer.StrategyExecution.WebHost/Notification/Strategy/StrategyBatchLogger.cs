using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Strategy
{
    public class StrategyBatchLogger : BatchNotification<StrategyNotification>, IBatchNotification<StrategyNotification>
    {
        private readonly ILogger logger;

        public StrategyBatchLogger(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<StrategyBatchLogger>();

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
                    var strategyNotifications = notifications.OrderBy(n => n.Timestamp).ToList();

                    foreach (var strategyNotification in strategyNotifications)
                    {
                        logger.Log<StrategyNotification>(GetStepNotificationLogLevel(strategyNotification), strategyNotification.NotificationEvent, strategyNotification, null, null);
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

        private static LogLevel GetStepNotificationLogLevel(StrategyNotification strategyNotification)
        {
            return strategyNotification.NotificationLevel switch
            {
                NotificationLevel.Debug => LogLevel.Debug,
                NotificationLevel.Information => LogLevel.Information,
                NotificationLevel.Warning => LogLevel.Warning,
                NotificationLevel.Error => LogLevel.Error,
                _ => LogLevel.Information,
            };
        }
    }
}
