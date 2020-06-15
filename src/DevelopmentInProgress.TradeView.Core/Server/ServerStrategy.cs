using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Server
{
    public class ServerStrategy
    {
        public ServerStrategy()
        {
            Connections = new List<ServerStrategyConnection>();
        }

        public TradeStrategy.Strategy Strategy { get; set; }
        public string StartedBy { get; set; }
        public string StoppedBy { get; set; }
        public DateTime Started { get; set; }
        public DateTime Stopped { get; set; }
        public List<ServerStrategyConnection> Connections { get; private set; }
    }
}