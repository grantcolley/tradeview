using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    public class StrategyPerformance
    {
        public StrategyPerformance()
        {
            StrategySymbolPerformances = new List<StrategySymbolPerformance>();
        }

        public string Strategy { get; set; }
        public DateTime StartTime { get; set; }
        public List<StrategySymbolPerformance> StrategySymbolPerformances { get; set; }
    }
}