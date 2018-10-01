using DevelopmentInProgress.Wpf.Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevelopmentInProgress.Wpf.Common.Services
{
    public class StrategyService : IStrategyService
    {
        private string userStrategiesFile;

        public StrategyService()
        {
            userStrategiesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}_Strategies.txt");
        }

        public List<Strategy> GetStrategies()
        {
            if (File.Exists(userStrategiesFile))
            {
                var json = File.ReadAllText(userStrategiesFile);
                var strategies = JsonConvert.DeserializeObject<List<Strategy>>(json);
                return strategies;
            }

            return new List<Strategy>();
        }

        public Strategy GetStrategy(string strategyName)
        {
            Strategy strategy = null;

            if (File.Exists(userStrategiesFile))
            {
                var json = File.ReadAllText(userStrategiesFile);
                var strategies = JsonConvert.DeserializeObject<List<Strategy>>(json);
                strategy = strategies.FirstOrDefault(s => s.Name.Equals(strategyName));
            }

            return strategy;
        }

        public void SaveStrategy(Strategy strategy)
        {
            if (strategy == null)
            {
                return;
            }

            List<Strategy> strategies;

            if (File.Exists(userStrategiesFile))
            {
                var rjson = File.ReadAllText(userStrategiesFile);
                strategies = JsonConvert.DeserializeObject<List<Strategy>>(rjson);
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
            File.WriteAllText(userStrategiesFile, wjson);
        }

        public void DeleteStrategy(Strategy strategy)
        {
            if (File.Exists(userStrategiesFile))
            {
                var rjson = File.ReadAllText(userStrategiesFile);
                var strategies = JsonConvert.DeserializeObject<List<Strategy>>(rjson);

                var remove = strategies.FirstOrDefault(s => s.Name.Equals(strategy.Name));
                if (remove != null)
                {
                    strategies.Remove(remove);
                    var wjson = JsonConvert.SerializeObject(strategies);
                    File.WriteAllText(userStrategiesFile, wjson);
                }
            }
        }
    }
}