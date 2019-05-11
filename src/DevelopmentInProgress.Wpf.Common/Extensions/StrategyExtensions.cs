using DevelopmentInProgress.Wpf.Common.Model;
using System.Linq;

namespace DevelopmentInProgress.Wpf.Common.Extensions
{
    public static class StrategyExtensions
    {
        public static MarketView.Interface.Strategy.Strategy GetInterfaceStrategy(this Strategy strategy)
        {
            var interfaceStrategy = new MarketView.Interface.Strategy.Strategy
            {
                Id = strategy.Id,
                Name = strategy.Name,
                Status = strategy.Status,
                TargetAssembly = strategy.TargetAssembly.File,
                TargetType = strategy.TargetType,
                Parameters = strategy.Parameters
            };

            var subscriptions = strategy.StrategySubscriptions.Select(s => s.GetInterfaceStrategySubscription()).ToList();
            interfaceStrategy.StrategySubscriptions.AddRange(subscriptions);

            return interfaceStrategy;
        }
    }
}
