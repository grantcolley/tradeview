using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class ServerMonitorExtensions
    {
        public static ServerMonitor ToServerMonitor(this Core.Server.TradeServer tradeServer)
        {
            return new ServerMonitor
            {
                Name = tradeServer.Name,
                Uri = tradeServer.Uri,
                MaxDegreeOfParallelism = tradeServer.MaxDegreeOfParallelism,
                Enabled = tradeServer.Enabled
            };
        }
    }
}
