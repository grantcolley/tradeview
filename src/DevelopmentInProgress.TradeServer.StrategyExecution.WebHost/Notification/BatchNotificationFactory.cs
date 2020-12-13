namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification
{
    public abstract class BatchNotificationFactory<T> : IBatchNotificationFactory<T>
    {
        public abstract IBatchNotification<T> GetBatchNotifier(BatchNotificationType batchNotifierType);
    }
}
