namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification
{
    public interface IBatchNotification<T>
    {
        void AddNotification(T item);
    }
}
