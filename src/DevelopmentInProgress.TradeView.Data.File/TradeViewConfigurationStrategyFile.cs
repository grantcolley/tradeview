using DevelopmentInProgress.TradeView.Interface.Strategy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Data.File
{
    public class TradeViewConfigurationStrategyFile : ITradeViewConfigurationStrategy
    {
        private readonly string userStrategiesFile;

        public TradeViewConfigurationStrategyFile()
        {
            userStrategiesFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}_Strategies.txt");
        }

        public async Task<List<StrategyConfig>> GetStrategiesAsync()
        {
            if (System.IO.File.Exists(userStrategiesFile))
            {
                using (var reader = System.IO.File.OpenText(userStrategiesFile))
                {
                    var json = await reader.ReadToEndAsync();
                    var strategies = JsonConvert.DeserializeObject<List<StrategyConfig>>(json);
                    return strategies;
                }
            }

            return new List<StrategyConfig>();
        }

        public async Task<StrategyConfig> GetStrategyAsync(string strategyName)
        {
            StrategyConfig strategy = null;

            if (System.IO.File.Exists(userStrategiesFile))
            {
                using (var reader = System.IO.File.OpenText(userStrategiesFile))
                {
                    var json = await reader.ReadToEndAsync();
                    var strategies = JsonConvert.DeserializeObject<List<StrategyConfig>>(json);
                    strategy = strategies.FirstOrDefault(s => s.Name.Equals(strategyName));
                }
            }

            return strategy;
        }

        public async Task SaveStrategyAsync(StrategyConfig strategyConfig)
        {
            if (strategyConfig == null)
            {
                return;
            }

            List<StrategyConfig> strategies;

            if (System.IO.File.Exists(userStrategiesFile))
            {
                using (var reader = System.IO.File.OpenText(userStrategiesFile))
                {
                    var rjson = await reader.ReadToEndAsync();
                    strategies = JsonConvert.DeserializeObject<List<StrategyConfig>>(rjson);
                }
            }
            else
            {
                strategies = new List<StrategyConfig>();
            }

            var dupe = strategies.FirstOrDefault(s => s.Name.Equals(strategyConfig.Name));
            if (dupe != null)
            {
                strategies.Remove(dupe);
            }

            strategies.Add(strategyConfig);

            var wjson = JsonConvert.SerializeObject(strategies, Formatting.Indented);

            UnicodeEncoding encoding = new UnicodeEncoding();
            char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
            using (System.IO.StreamWriter writer = System.IO.File.CreateText(userStrategiesFile))
            {
                await writer.WriteAsync(chars, 0, chars.Length);
            }
        }

        public async Task DeleteStrategyAsync(StrategyConfig strategyConfig)
        {
            if (System.IO.File.Exists(userStrategiesFile))
            {
                List<StrategyConfig> strategies = null;

                using (var reader = System.IO.File.OpenText(userStrategiesFile))
                {
                    var rjson = await reader.ReadToEndAsync();
                    strategies = JsonConvert.DeserializeObject<List<StrategyConfig>>(rjson);
                }

                var remove = strategies.FirstOrDefault(s => s.Name.Equals(strategyConfig.Name));
                if (remove != null)
                {
                    strategies.Remove(remove);
                    var wjson = JsonConvert.SerializeObject(strategies);

                    UnicodeEncoding encoding = new UnicodeEncoding();
                    char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
                    using (System.IO.StreamWriter writer = System.IO.File.CreateText(userStrategiesFile))
                    {
                        await writer.WriteAsync(chars, 0, chars.Length);
                    }
                }
            }
        }
    }
}
