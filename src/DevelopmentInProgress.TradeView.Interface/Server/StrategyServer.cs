using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Server
{
    public class StrategyServer
    {
        public StrategyServer()
        {
            Strategies = new List<StrategyManager>();
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public string StartedBy { get; set; }
        public string StoppedBy { get; set; }
        public DateTime Started { get; set; }
        public DateTime Stopped { get; set; }
        public List<StrategyManager> Strategies { get; set; }
    }
}
