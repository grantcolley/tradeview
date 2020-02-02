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
    public class TradeViewConfigurationStrategyServerFile : ITradeViewConfigurationStrategyServer
    {
        private readonly string userStrategyServersFile;

        public TradeViewConfigurationStrategyServerFile()
        {
            userStrategyServersFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.UserName}_StrategyServers.txt");
        }

        public async Task<List<StrategyServer>> GetStrategyServersAsync()
        {
            if (System.IO.File.Exists(userStrategyServersFile))
            {
                using (var reader = System.IO.File.OpenText(userStrategyServersFile))
                {
                    var json = await reader.ReadToEndAsync();
                    var strategyServers = JsonConvert.DeserializeObject<List<StrategyServer>>(json);
                    return strategyServers;
                }
            }

            return new List<StrategyServer>
            {
                GetDemoStrategyServer()
            };
        }

        public async Task<StrategyServer> GetStrategyServerAsync(string strategyServerName)
        {
            StrategyServer strategyServer = null;

            if (System.IO.File.Exists(userStrategyServersFile))
            {
                using (var reader = System.IO.File.OpenText(userStrategyServersFile))
                {
                    var json = await reader.ReadToEndAsync();
                    var strategyServers = JsonConvert.DeserializeObject<List<StrategyServer>>(json);
                    strategyServer = strategyServers.FirstOrDefault(s => s.Name.Equals(strategyServerName));
                    return strategyServer;
                }
            }

            return GetDemoStrategyServer();
        }

        public async Task SaveStrategyServerAsync(StrategyServer strategyServer)
        {
            if (strategyServer == null)
            {
                return;
            }

            List<StrategyServer> strategyServers;

            if (System.IO.File.Exists(userStrategyServersFile))
            {
                using (var reader = System.IO.File.OpenText(userStrategyServersFile))
                {
                    var rjson = await reader.ReadToEndAsync();
                    strategyServers = JsonConvert.DeserializeObject<List<StrategyServer>>(rjson);
                }
            }
            else
            {
                strategyServers = new List<StrategyServer>();
            }

            var dupe = strategyServers.FirstOrDefault(s => s.Name.Equals(strategyServer.Name));
            if (dupe != null)
            {
                strategyServers.Remove(dupe);
            }

            strategyServers.Add(strategyServer);

            var wjson = JsonConvert.SerializeObject(strategyServers, Formatting.Indented);

            UnicodeEncoding encoding = new UnicodeEncoding();
            char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
            using (StreamWriter writer = System.IO.File.CreateText(userStrategyServersFile))
            {
                await writer.WriteAsync(chars, 0, chars.Length);
            }
        }

        public async Task DeleteStrategyServerAsync(StrategyServer strategyServer)
        {
            if (System.IO.File.Exists(userStrategyServersFile))
            {
                List<StrategyServer> strategyServers = null;

                using (var reader = System.IO.File.OpenText(userStrategyServersFile))
                {
                    var rjson = await reader.ReadToEndAsync();
                    strategyServers = JsonConvert.DeserializeObject<List<StrategyServer>>(rjson);
                }

                var remove = strategyServers.FirstOrDefault(s => s.Name.Equals(strategyServer.Name));
                if (remove != null)
                {
                    strategyServers.Remove(remove);
                    var wjson = JsonConvert.SerializeObject(strategyServers);

                    UnicodeEncoding encoding = new UnicodeEncoding();
                    char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
                    using (StreamWriter writer = System.IO.File.CreateText(userStrategyServersFile))
                    {
                        await writer.WriteAsync(chars, 0, chars.Length);
                    }
                }
            }
        }

        private StrategyServer GetDemoStrategyServer()
        {
            return new StrategyServer
            {
                Name = "Strategy Server #1",
                Url = "http://localhost:5500"
            };
        }
    }
}
