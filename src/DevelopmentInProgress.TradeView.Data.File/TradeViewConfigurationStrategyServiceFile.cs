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
    public class TradeViewConfigurationStrategyServiceFile : ITradeViewConfigurationStrategyService
    {
        private readonly string userStrategyServicesFile;

        public TradeViewConfigurationStrategyServiceFile()
        {
            userStrategyServicesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}_StrategyServices.txt");
        }

        public async Task<List<StrategyService>> GetStrategyServicesAsync()
        {
            if (System.IO.File.Exists(userStrategyServicesFile))
            {
                using (var reader = System.IO.File.OpenText(userStrategyServicesFile))
                {
                    var json = await reader.ReadToEndAsync();
                    var strategyServices = JsonConvert.DeserializeObject<List<StrategyService>>(json);
                    return strategyServices;
                }
            }

            return new List<StrategyService>
            {
                GetDemoStrategyService()
            };
        }

        public async Task<StrategyService> GetStrategyServiceAsync(string strategyServiceName)
        {
            StrategyService strategyService = null;

            if (System.IO.File.Exists(userStrategyServicesFile))
            {
                using (var reader = System.IO.File.OpenText(userStrategyServicesFile))
                {
                    var json = await reader.ReadToEndAsync();
                    var strategyServices = JsonConvert.DeserializeObject<List<StrategyService>>(json);
                    strategyService = strategyServices.FirstOrDefault(s => s.Name.Equals(strategyServiceName));
                    return strategyService;
                }
            }

            return GetDemoStrategyService();
        }

        public async Task SaveStrategyServiceAsync(StrategyService strategyService)
        {
            if (strategyService == null)
            {
                return;
            }

            List<StrategyService> strategyServices;

            if (System.IO.File.Exists(userStrategyServicesFile))
            {
                using (var reader = System.IO.File.OpenText(userStrategyServicesFile))
                {
                    var rjson = await reader.ReadToEndAsync();
                    strategyServices = JsonConvert.DeserializeObject<List<StrategyService>>(rjson);
                }
            }
            else
            {
                strategyServices = new List<StrategyService>();
            }

            var dupe = strategyServices.FirstOrDefault(s => s.Name.Equals(strategyService.Name));
            if (dupe != null)
            {
                strategyServices.Remove(dupe);
            }

            strategyServices.Add(strategyService);

            var wjson = JsonConvert.SerializeObject(strategyServices, Formatting.Indented);

            UnicodeEncoding encoding = new UnicodeEncoding();
            char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
            using (StreamWriter writer = System.IO.File.CreateText(userStrategyServicesFile))
            {
                await writer.WriteAsync(chars, 0, chars.Length);
            }
        }

        public async Task DeleteStrategyServiceAsync(StrategyService strategyService)
        {
            if (System.IO.File.Exists(userStrategyServicesFile))
            {
                List<StrategyService> strategyServices = null;

                using (var reader = System.IO.File.OpenText(userStrategyServicesFile))
                {
                    var rjson = await reader.ReadToEndAsync();
                    strategyServices = JsonConvert.DeserializeObject<List<StrategyService>>(rjson);
                }

                var remove = strategyServices.FirstOrDefault(s => s.Name.Equals(strategyService.Name));
                if (remove != null)
                {
                    strategyServices.Remove(remove);
                    var wjson = JsonConvert.SerializeObject(strategyServices);

                    UnicodeEncoding encoding = new UnicodeEncoding();
                    char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
                    using (StreamWriter writer = System.IO.File.CreateText(userStrategyServicesFile))
                    {
                        await writer.WriteAsync(chars, 0, chars.Length);
                    }
                }
            }
        }

        private StrategyService GetDemoStrategyService()
        {
            return new StrategyService
            {
                Name = "Strategy Service #1",
                Url = "http://localhost:5500"
            };
        }
    }
}
