using System;
using System.Collections.Generic;
using System.Text;

namespace DevelopmentInProgress.TradeView.Interface.Strategy
{
    public class StrategyConfig
    {
        public StrategyConfig()
        {
            StrategySubscriptions = new List<StrategySubscription>();
            Dependencies = new List<string>();
            DisplayDependencies = new List<string>();
        }

        public string Name { get; set; }
        public string TargetAssembly { get; set; }
        public string DisplayAssembly { get; set; }
        public string Parameters { get; set; }
        public StrategyStatus Status { get; set; }
        public List<StrategySubscription> StrategySubscriptions { get; set; }
        public List<string> Dependencies { get; set; }
        public List<string> DisplayDependencies { get; set; }
        public string TargetType { get; set; }
        public string DisplayViewType { get; set; }
        public string DisplayViewModelType { get; set; }
        public int TradesChartDisplayCount { get; set; }
        public int TradesDisplayCount { get; set; }
        public int OrderBookChartDisplayCount { get; set; }
        public int OrderBookDisplayCount { get; set; }
        public string StrategyServerUrl { get; set; }
    }
}
