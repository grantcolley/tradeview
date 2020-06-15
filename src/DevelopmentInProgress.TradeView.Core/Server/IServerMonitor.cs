using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Server
{
    public interface IServerMonitor : ITradeServer
    {
        string StartedBy { get; set; }
        string StoppedBy { get; set; }
        DateTime Started { get; set; }
        DateTime Stopped { get; set; }
        List<ServerStrategy> Strategies { get; }
        ServerNotification GetServerNotification(List<ServerStrategy> strategies);
    }
}
