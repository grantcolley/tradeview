using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Model;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            userStrategiesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}_Strategies.txt");
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

            return new List<StrategyConfig> 
            {
                GetDemoStrategyConfig()
            };
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
                    return strategy;
                }
            }

            return GetDemoStrategyConfig();
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
            using (StreamWriter writer = System.IO.File.CreateText(userStrategiesFile))
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
                    using (StreamWriter writer = System.IO.File.CreateText(userStrategiesFile))
                    {
                        await writer.WriteAsync(chars, 0, chars.Length);
                    }
                }
            }
        }

        private StrategyConfig GetDemoStrategyConfig()
        {
            return new StrategyConfig
            {
                Name = "Demo - ETHBTC",
                TargetType = "DevelopmentInProgress.Strategy.Demo.DemoTradeStrategy",
                TargetAssembly = Path.Combine(Environment.CurrentDirectory, "DevelopmentInProgress.Strategy.Demo.dll"),
                DisplayViewType = "DevelopmentInProgress.Strategy.Demo.Wpf.View.DemoView",
                DisplayViewModelType = "DevelopmentInProgress.Strategy.Demo.Wpf.ViewModel.DemoViewModel",
                DisplayAssembly = Path.Combine(Environment.CurrentDirectory, "DevelopmentInProgress.Strategy.Demo.Wpf.dll"),
                Parameters = "{\r\n  \"BuyIndicator\": 0.00015,\r\n  \"SellIndicator\": 0.00015,\r\n  \"TradeMovingAvarageSetLength\": 0,\r\n  \"Suspend\": true,\r\n  \"StrategyName\": \"Demo - ETHBTC\",\r\n  \"Value\": null\r\n}",
                TradesChartDisplayCount = 1000,
                TradesDisplayCount = 14,
                OrderBookChartDisplayCount = 20,
                OrderBookDisplayCount = 9,
                StrategyServerUrl = "http://localhost:5500",
                StrategySubscriptions = new List<StrategySubscription>
                {
                    new StrategySubscription
                    {
                        Symbol = "ETHBTC",
                        Limit = 0,
                        Exchange = Exchange.Binance,
                        Subscribe = Subscribe.Trades | Subscribe.OrderBook | Subscribe.Candlesticks,
                        CandlestickInterval = CandlestickInterval.Minute
                    }
                },
                Dependencies = new List<string> 
                {
                    Path.Combine(Environment.CurrentDirectory, "DevelopmentInProgress.TradeView.Interface.dll"),
                    Path.Combine(Environment.CurrentDirectory, "DevelopmentInProgress.Strategy.Demo.dll")
                },
                DisplayDependencies = new List<string>
                {
                    Path.Combine(Environment.CurrentDirectory, "DevelopmentInProgress.TradeView.Interface.dll"),
                    Path.Combine(Environment.CurrentDirectory, "DevelopmentInProgress.Strategy.Demo.Wpf.dll"),
                    Path.Combine(Environment.CurrentDirectory, "DevelopmentInProgress.Strategy.Demo.dll")
                }
            };
        }
    }
}
