using DevelopmentInProgress.TradeView.Interface.Strategy;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Server
{
    public class StrategyManager
    {
        public StrategyManager()
        {
            Connections = new List<StrategyConnection>();
        }

        public Strategy.Strategy Strategy { get; set; }
        public ITradeStrategy TradeStrategy { get; set; }
        public string StartedBy { get; set; }
        public string StoppedBy { get; set; }
        public DateTime Started { get; set; }
        public DateTime Stopped { get; set; }
        public List<StrategyConnection> Connections { get; set; }
    }
}