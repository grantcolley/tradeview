using DevelopmentInProgress.Wpf.Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.Common.Services
{
    public class StrategyService : IStrategyService
    {
        private string userStrategiesFile;

        public StrategyService()
        {
            userStrategiesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}_Strategies.txt");
        }

        public async Task<List<Strategy>> GetStrategies()
        {
            if (File.Exists(userStrategiesFile))
            {
                using (var reader = File.OpenText(userStrategiesFile))
                {
                    var json = await reader.ReadToEndAsync();
                    var strategies = JsonConvert.DeserializeObject<List<Strategy>>(json);
                    return strategies;
                }
            }

            return new List<Strategy>();
        }

        public async Task<Strategy> GetStrategy(string strategyName)
        {
            Strategy strategy = null;

            if (File.Exists(userStrategiesFile))
            {
                using (var reader = File.OpenText(userStrategiesFile))
                {
                    var json = await reader.ReadToEndAsync();
                    var strategies = JsonConvert.DeserializeObject<List<Strategy>>(json);
                    strategy = strategies.FirstOrDefault(s => s.Name.Equals(strategyName));
                }
            }

            return strategy;
        }

        public async Task SaveStrategy(Strategy strategy)
        {
            if (strategy == null)
            {
                return;
            }

            List<Strategy> strategies;

            if (File.Exists(userStrategiesFile))
            {
                using (var reader = File.OpenText(userStrategiesFile))
                {
                    var rjson = await reader.ReadToEndAsync();
                    strategies = JsonConvert.DeserializeObject<List<Strategy>>(rjson);
                }
            }
            else
            {
                strategies = new List<Strategy>();
            }

            var dupe = strategies.FirstOrDefault(s => s.Name.Equals(strategy.Name));
            if (dupe != null)
            {
                strategies.Remove(dupe);
            }

            strategies.Add(strategy);

            var wjson = JsonConvert.SerializeObject(strategies);
            
            UnicodeEncoding encoding = new UnicodeEncoding();
            char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
            using (StreamWriter writer = File.CreateText(userStrategiesFile))
            {
                await writer.WriteAsync(chars, 0, chars.Length);
            }
        }

        public async Task DeleteStrategy(Strategy strategy)
        {
            if (File.Exists(userStrategiesFile))
            {
                List<Strategy> strategies = null;

                using (var reader = File.OpenText(userStrategiesFile))
                {
                    var rjson = await reader.ReadToEndAsync();
                    strategies = JsonConvert.DeserializeObject<List<Strategy>>(rjson);
                }

                var remove = strategies.FirstOrDefault(s => s.Name.Equals(strategy.Name));
                if (remove != null)
                {
                    strategies.Remove(remove);
                    var wjson = JsonConvert.SerializeObject(strategies);

                    UnicodeEncoding encoding = new UnicodeEncoding();
                    char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
                    using (StreamWriter writer = File.CreateText(userStrategiesFile))
                    {
                        await writer.WriteAsync(chars, 0, chars.Length);
                    }
                }
            }
        }
    }
}