using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Linq;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class StrategyExtensions
    {
        public static Interface.Strategy.Strategy ToInterfaceStrategy(this Strategy strategy)
        {
            var interfaceStrategy = new Interface.Strategy.Strategy
            {
                Id = strategy.Id,
                Name = strategy.Name,
                Status = strategy.Status,
                TargetAssembly = strategy.TargetAssembly.DisplayName,
                TargetType = strategy.TargetType,
                Parameters = strategy.Parameters
            };

            var subscriptions = strategy.StrategySubscriptions.Select(s => s.ToInterfaceStrategySubscription()).ToList();
            interfaceStrategy.StrategySubscriptions.AddRange(subscriptions);

            return interfaceStrategy;
        }

        public static Interface.Strategy.StrategyConfig ToInterfaceStrategyConfig(this Strategy strategy)
        {
            var strategyConfig = new Interface.Strategy.StrategyConfig
            {
                Id = strategy.Id,
                Name = strategy.Name,
                Status = strategy.Status,
                TargetAssembly = strategy.TargetAssembly?.File,
                TargetType = strategy.TargetType,
                Parameters = strategy.Parameters,
                DisplayAssembly = strategy.DisplayAssembly?.File,
                DisplayViewType = strategy.DisplayViewType,
                DisplayViewModelType = strategy.DisplayViewModelType,
                TradesChartDisplayCount = strategy.TradesChartDisplayCount,
                TradesDisplayCount = strategy.TradesDisplayCount,
                OrderBookChartDisplayCount = strategy.OrderBookChartDisplayCount,
                OrderBookDisplayCount = strategy.OrderBookDisplayCount,
                Dependencies = strategy.Dependencies.Select(f => f.File).ToList(),
                DisplayDependencies = strategy.DisplayDependencies.Select(f=> f.File).ToList()
            };
            
            var subscriptions = strategy.StrategySubscriptions.Select(s => s.ToInterfaceStrategySubscription()).ToList();
            strategyConfig.StrategySubscriptions.AddRange(subscriptions);

            return strategyConfig;
        }

        public static Strategy ToWpfStrategy(this Interface.Strategy.StrategyConfig strategyConfig)
        {
            var strategy = new Strategy()
            {
                Id = strategyConfig.Id,
                Name = strategyConfig.Name,
                Status = strategyConfig.Status,
                TargetAssembly = new StrategyFile { File = strategyConfig.TargetAssembly, FileType = StrategyFileType.StrategyFile },
                TargetType = strategyConfig.TargetType,
                Parameters = strategyConfig.Parameters,
                DisplayAssembly = new StrategyFile { File = strategyConfig.DisplayAssembly, FileType = StrategyFileType.DisplayFile },
                DisplayViewType = strategyConfig.DisplayViewType,
                DisplayViewModelType = strategyConfig.DisplayViewModelType,
                TradesChartDisplayCount = strategyConfig.TradesChartDisplayCount,
                TradesDisplayCount = strategyConfig.TradesDisplayCount,
                OrderBookChartDisplayCount = strategyConfig.OrderBookChartDisplayCount,
                OrderBookDisplayCount = strategyConfig.OrderBookDisplayCount
            };

            var subscriptions = strategyConfig.StrategySubscriptions.Select(s => s.ToWpfStrategySubscription()).ToList();

            foreach(var s in subscriptions)
            {
                strategy.StrategySubscriptions.Add(s);
            }

            foreach (var f in strategyConfig.Dependencies)
            {
                strategy.Dependencies.Add(new StrategyFile { File = f, FileType = StrategyFileType.StrategyFile });
            }

            foreach (var f in strategyConfig.DisplayDependencies)
            {
                strategy.DisplayDependencies.Add(new StrategyFile { File = f, FileType = StrategyFileType.DisplayFile });
            }

            return strategy;
        }
    }
}
