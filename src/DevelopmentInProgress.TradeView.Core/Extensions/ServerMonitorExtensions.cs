using DevelopmentInProgress.TradeView.Core.Server;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Extensions
{
    public static class ServerMonitorExtensions
    {
        public static ServerNotification GetNotification(this ServerMonitor serverMonitor, List<ServerStrategy> serverStrategies)
        {
            if (serverMonitor == null)
            {
                throw new ArgumentNullException(nameof(serverMonitor));
            }

            var clone = new ServerMonitor
            {
                Name = serverMonitor.Name,
                Url = serverMonitor.Url,
                MaxDegreeOfParallelism = serverMonitor.MaxDegreeOfParallelism,
                StartedBy = serverMonitor.StartedBy,
                Started = serverMonitor.Started,
                StoppedBy = serverMonitor.StoppedBy,
                Stopped = serverMonitor.Stopped                
            };

            clone.Strategies.AddRange(serverStrategies);

            return new ServerNotification
            {
                Machine = Environment.MachineName,
                Message = clone.ToString()
            };
        }
    }
}
