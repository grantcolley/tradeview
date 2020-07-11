using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;
using System.Linq;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class StrategyExtensions
    {
        public static Core.TradeStrategy.Strategy ToCoreStrategy(this Strategy strategy)
        {
            if(strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            var interfaceStrategy = new Core.TradeStrategy.Strategy
            {
                Id = strategy.Id,
                Name = strategy.Name,
                Status = strategy.Status,
                TargetAssembly = strategy.TargetAssembly.DisplayName,
                TargetType = strategy.TargetType,
                Parameters = strategy.Parameters
            };

            var subscriptions = strategy.StrategySubscriptions.Select(s => s.ToCoreStrategySubscription()).ToList();
            interfaceStrategy.StrategySubscriptions.AddRange(subscriptions);

            return interfaceStrategy;
        }

        public static Core.TradeStrategy.StrategyConfig ToCoreStrategyConfig(this Strategy strategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            var strategyConfig = new Core.TradeStrategy.StrategyConfig
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
            };

            strategyConfig.Dependencies.AddRange(strategy.Dependencies.Select(f => f.File).ToList());
            strategyConfig.DisplayDependencies.AddRange(strategy.DisplayDependencies.Select(f => f.File).ToList());

            var subscriptions = strategy.StrategySubscriptions.Select(s => s.ToCoreStrategySubscription()).ToList();
            strategyConfig.StrategySubscriptions.AddRange(subscriptions);

            return strategyConfig;
        }

        public static Strategy ToWpfStrategy(this Core.TradeStrategy.StrategyConfig strategyConfig)
        {
            if (strategyConfig == null)
            {
                throw new ArgumentNullException(nameof(strategyConfig));
            }

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
