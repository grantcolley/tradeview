using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.TradeStrategy
{
    public class Strategy
    {
        public Strategy()
        {
            StrategySubscriptions = new List<StrategySubscription>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public StrategyStatus Status { get; set; }
        public List<StrategySubscription> StrategySubscriptions { get; set; }

        /// <summary>
        /// The assembly to load and run. The assembly must contain a class that implements
        /// interface <see cref="DevelopmentInProgress.TradeServer.TradeStrategy.ITradeStrategy"/>
        /// </summary>
        public string TargetAssembly { get; set; }

        /// <summary>
        /// The type name to run in the <see cref="TargetAssembly"/> including full namespace.
        /// The target type must implement <see cref="DevelopmentInProgress.TradeServer.TradeStrategy.ITradeStrategy"/>
        /// </summary>
        public string TargetType { get; set; }
        public string Tag { get; set; }
    }
}