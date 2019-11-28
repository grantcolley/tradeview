using System;
using System.Text;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using Newtonsoft.Json;

namespace DevelopmentInProgress.TradeView.Data.File
{
    public class TradeViewStrategyPerformance : ITradeViewStrategyPerformance
    {
        public async Task<StrategyPerformance> GetStrategyPerformance(string strategyName)
        {
            StrategyPerformance strategyPerformance = null;

            var strategyPerformanceFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}_{strategyName}.txt");

            if (System.IO.File.Exists(strategyPerformanceFile))
            {
                using (var reader = System.IO.File.OpenText(strategyPerformanceFile))
                {
                    var json = await reader.ReadToEndAsync();
                    strategyPerformance = JsonConvert.DeserializeObject<StrategyPerformance>(json);
                }
            }
            else
            {
                strategyPerformance = new StrategyPerformance { Strategy = strategyName };
                await SaveStrategyPerformance(strategyPerformance).ConfigureAwait(false);
            }

            return strategyPerformance;
        }

        public async Task SaveStrategyPerformance(StrategyPerformance strategyPerformance)
        {
            if (strategyPerformance == null)
            {
                return;
            }

            var strategyPerformanceFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}_{strategyPerformance.Strategy}.txt");

            var json = JsonConvert.SerializeObject(strategyPerformance, Formatting.Indented);

            UnicodeEncoding encoding = new UnicodeEncoding();
            char[] chars = encoding.GetChars(encoding.GetBytes(json));
            using (System.IO.StreamWriter writer = System.IO.File.CreateText(strategyPerformanceFile))
            {
                await writer.WriteAsync(chars, 0, chars.Length);
            }
        }
    }
}
