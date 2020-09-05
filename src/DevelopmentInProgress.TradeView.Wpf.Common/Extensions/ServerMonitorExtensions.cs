using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;
using System.Net.Http;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class ServerMonitorExtensions
    {
        public static ServerMonitor ToServerMonitor(this Core.Server.TradeServer tradeServer, HttpClient httpClient)
        {
            if (tradeServer == null)
            {
                throw new ArgumentNullException(nameof(tradeServer));
            }

            return new ServerMonitor(httpClient)
            {
                Name = tradeServer.Name,
                Uri = tradeServer.Uri,
                MaxDegreeOfParallelism = tradeServer.MaxDegreeOfParallelism,
                Enabled = tradeServer.Enabled
            };
        }
    }
}
