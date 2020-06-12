using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Server
{
    public interface IServerMonitor : IServer
    {
        string StartedBy { get; set; }
        string StoppedBy { get; set; }
        DateTime Started { get; set; }
        DateTime Stopped { get; set; }
        List<ServerStrategy> Strategies { get; set; }
        ServerNotification GetServerNotification(List<ServerStrategy> strategies);
    }
}
