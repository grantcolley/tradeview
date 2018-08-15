using DevelopmentInProgress.Wpf.StrategyManager.Model;
using System.Linq;

namespace DevelopmentInProgress.Wpf.StrategyManager.Extensions
{
    public static class StrategyExtensions
    {
        public static MarketView.Interface.TradeStrategy.Strategy GetInterfaceStrategy(this Strategy strategy)
        {
            var interfaceStrategy = new MarketView.Interface.TradeStrategy.Strategy
            {
                Id = strategy.Id,
                Name = strategy.Name,
                Status = strategy.Status,
                TargetAssembly = strategy.TargetAssembly.File,
                TargetType = strategy.TargetType,
                Tag = strategy.Tag
            };

            var subscriptions = strategy.StrategySubscriptions.Select(s => s.GetInterfaceStrategySubscription()).ToList();
            interfaceStrategy.StrategySubscriptions.AddRange(subscriptions);

            return interfaceStrategy;
        }
    }
}
