using DevelopmentInProgress.MarketView.Interface.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    public class StrategyPerformance
    {
        public StrategyPerformance()
        {
            StrategyPerformanceIndicators = new List<StrategyPerformanceIndicator>();
            StrategyTrades = new List<AccountTrade>();
        }

        public string Strategy { get; set; }
        public List<StrategyPerformanceIndicator> StrategyPerformanceIndicators { get; set; }
        public List<AccountTrade> StrategyTrades { get; set; }
    }
}