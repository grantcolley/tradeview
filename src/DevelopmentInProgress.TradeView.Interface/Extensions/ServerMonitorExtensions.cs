using DevelopmentInProgress.TradeView.Interface.Server;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Extensions
{
    public static class ServerMonitorExtensions
    {
        public static ServerNotification GetNotification(this ServerMonitor serverMonitor, List<ServerStrategy> serverStrategies)
        {
            var clone = new ServerMonitor
            {
                Name = serverMonitor.Name,
                Url = serverMonitor.Url,
                MaxDegreeOfParallelism = serverMonitor.MaxDegreeOfParallelism,
                StartedBy = serverMonitor.StartedBy,
                Started = serverMonitor.Started,
                StoppedBy = serverMonitor.StoppedBy,
                Stopped = serverMonitor.Stopped,
                Strategies = serverStrategies
            };

            return new ServerNotification
            {
                Machine = Environment.MachineName,
                Message = clone.ToString()
            };
        }
    }
}
