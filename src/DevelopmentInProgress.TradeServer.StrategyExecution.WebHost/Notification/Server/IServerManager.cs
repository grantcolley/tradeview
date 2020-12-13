using DevelopmentInProgress.TradeView.Core.Server;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Server
{
    public interface IServerManager
    {
        IServerMonitor ServerMonitor { get; }
        void Shutdown();
    }
}
