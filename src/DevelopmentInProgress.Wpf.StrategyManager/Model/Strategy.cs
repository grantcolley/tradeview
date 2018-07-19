using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.Wpf.Common.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.StrategyManager.Model
{
    public class Strategy : EntityBase
    {
        public Strategy()
        {
            StrategySubscriptions = new List<StrategySubscription>();
        }

        public string Name { get; set; }
        public StrategyStatus Status { get; set; }
        public List<StrategySubscription> StrategySubscriptions { get; set; }
        public string TargetAssembly { get; set; }
        public string TargetType { get; set; }
        public string Tag { get; set; }
    }
}
