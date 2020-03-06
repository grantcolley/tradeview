using DevelopmentInProgress.TradeView.Interface.Server;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Extensions
{
    public static class ServerExtensions
    {
        public static ServerNotification GetNotification(this Server.Server server, List<ServerStrategy> serverStrategies)
        {
            var clone = new Server.Server
            {
                Name = server.Name,
                Url = server.Url,
                MaxDegreeOfParallelism = server.MaxDegreeOfParallelism,
                StartedBy = server.StartedBy,
                Started = server.Started,
                StoppedBy = server.StoppedBy,
                Stopped = server.Stopped,
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
