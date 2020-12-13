using System;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Server
{
    public interface IServerNotification
    {
        event EventHandler<ServerNotificationEventArgs> ServerNotification;
    }
}
