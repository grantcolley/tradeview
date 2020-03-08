using DevelopmentInProgress.TradeView.Wpf.Dashboard.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.Extensions
{
    public static class ServerMonitorExtensions
    {
        public static ServerMonitor ToServerMonitor(this Interface.Server.Server server)
        {
            return new ServerMonitor
            {
                Name = server.Name,
                Url = server.Url,
                MaxDegreeOfParallelism = server.MaxDegreeOfParallelism
            };
        }
    }
}
