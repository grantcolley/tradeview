using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class ServerMonitorExtensions
    {
        public static ServerMonitor ToServerMonitor(this Interface.Server.Server server)
        {
            return new ServerMonitor
            {
                Name = server.Name,
                Url = server.Url,
                MaxDegreeOfParallelism = server.MaxDegreeOfParallelism,
                Enabled = server.Enabled
            };
        }
    }
}
