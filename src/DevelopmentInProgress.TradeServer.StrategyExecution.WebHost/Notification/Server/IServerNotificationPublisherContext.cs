using DevelopmentInProgress.TradeView.Core.Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Server
{
    public interface IServerNotificationPublisherContext
    {
        Task PublishNotificationsAsync(IEnumerable<ServerNotification> notifications);
    }
}
