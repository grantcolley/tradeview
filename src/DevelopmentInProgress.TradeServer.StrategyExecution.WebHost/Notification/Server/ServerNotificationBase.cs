using System;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Server
{
    public abstract class ServerNotificationBase : IServerNotification
    {
        public event EventHandler<ServerNotificationEventArgs> ServerNotification;

        protected void OnServerNotification()
        {
            var serverNotification = ServerNotification;
            serverNotification?.Invoke(this, new ServerNotificationEventArgs());
        }
    }
}
