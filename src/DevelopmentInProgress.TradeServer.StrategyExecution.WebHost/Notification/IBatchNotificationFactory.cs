namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification
{
    public interface IBatchNotificationFactory<T>
    {
        IBatchNotification<T> GetBatchNotifier(BatchNotificationType batchNotifierType);
    }
}