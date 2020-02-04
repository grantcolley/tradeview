using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Server
{
    public class StrategyStatus
    {
        public StrategyStatus()
        {
            Connections = new List<StrategyConnection>();
        }

        public Strategy.Strategy Strategy { get; set; }
        public string StartedBy { get; set; }
        public string StoppedBy { get; set; }
        public DateTime Started { get; set; }
        public DateTime Stopped { get; set; }
        public List<StrategyConnection> Connections { get; set; }
    }
}