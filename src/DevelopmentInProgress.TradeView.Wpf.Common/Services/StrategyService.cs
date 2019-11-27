using DevelopmentInProgress.TradeView.Data;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public class StrategyService : IStrategyService
    {
        private readonly ITradeViewConfigurationStrategy configurationStrategy;

        public StrategyService(ITradeViewConfigurationStrategy configurationStrategy)
        {
            this.configurationStrategy = configurationStrategy;
        }

        public async Task<List<Strategy>> GetStrategies()
        {
            var result = await configurationStrategy.GetStrategiesAsync();
            return result.Select(s => s.ToWpfStrategy()).ToList();
        }

        public async Task<Strategy> GetStrategy(string strategyName)
        {
            var result = await configurationStrategy.GetStrategyAsync(strategyName);
            return result.ToWpfStrategy();
        }

        public Task SaveStrategy(Strategy strategy)
        {
            return configurationStrategy.SaveStrategyAsync(strategy.ToInterfaceStrategyConfig());
        }

        public Task DeleteStrategy(Strategy strategy)
        {
            return configurationStrategy.DeleteStrategyAsync(strategy.ToInterfaceStrategyConfig());
        }

        public async Task<Interface.Strategy.StrategyPerformance> GetStrategyPerformance(string strategyName)
        {
            Interface.Strategy.StrategyPerformance strategyPerformance = null;

            var strategyPerformanceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}_{strategyName}.txt");

            if (File.Exists(strategyPerformanceFile))
            {
                using (var reader = File.OpenText(strategyPerformanceFile))
                {
                    var json = await reader.ReadToEndAsync();
                    strategyPerformance = JsonConvert.DeserializeObject<Interface.Strategy.StrategyPerformance>(json);
                }
            }
            else
            {
                strategyPerformance = new Interface.Strategy.StrategyPerformance { Strategy = strategyName };
                await SaveStrategyPerformance(strategyPerformance).ConfigureAwait(false);
            }

            return strategyPerformance;
        }

        public async Task SaveStrategyPerformance(Interface.Strategy.StrategyPerformance strategyPerformance)
        {
            if(strategyPerformance == null)
            {
                return;
            }

            var strategyPerformanceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}_{strategyPerformance.Strategy}.txt");

            var json = JsonConvert.SerializeObject(strategyPerformance, Formatting.Indented);

            UnicodeEncoding encoding = new UnicodeEncoding();
            char[] chars = encoding.GetChars(encoding.GetBytes(json));
            using (StreamWriter writer = File.CreateText(strategyPerformanceFile))
            {
                await writer.WriteAsync(chars, 0, chars.Length);
            }
        }
    }
}