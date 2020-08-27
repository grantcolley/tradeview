using DevelopmentInProgress.TradeView.Core.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Data.File
{
    public class TradeViewConfigurationServerFile : ITradeViewConfigurationServer
    {
        private readonly string userServersFile;

        public TradeViewConfigurationServerFile()
        {
            userServersFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.MachineName}_TradeServers.txt");
        }

        public Task<ServerConfiguration> GetServerConfiguration()
        {
            return Task.FromResult(new ServerConfiguration { ObserveServerInterval = 10 });
        }

        public async Task<List<TradeServer>> GetTradeServersAsync()
        {
            if (System.IO.File.Exists(userServersFile))
            {
                using var reader = System.IO.File.OpenText(userServersFile);
                var json = await reader.ReadToEndAsync().ConfigureAwait(false);
                var Servers = JsonConvert.DeserializeObject<List<TradeServer>>(json);
                return Servers.Where(s => !string.IsNullOrWhiteSpace(s.Name)
                                        && !string.IsNullOrWhiteSpace(s.Uri.OriginalString)).ToList();
            }

            return new List<TradeServer>
            {
                GetDemoTradeServer()
            };
        }

        public async Task<TradeServer> GetTradeServerAsync(string serverName)
        {
            TradeServer server = null;

            if (System.IO.File.Exists(userServersFile))
            {
                using var reader = System.IO.File.OpenText(userServersFile);
                var json = await reader.ReadToEndAsync().ConfigureAwait(false);
                var servers = JsonConvert.DeserializeObject<List<TradeServer>>(json);
                server = servers.FirstOrDefault(s => s.Name.Equals(serverName, StringComparison.Ordinal));
                return server;
            }

            return GetDemoTradeServer();
        }

        public async Task SaveTradeServerAsync(TradeServer server)
        {
            if (server == null)
            {
                return;
            }

            List<TradeServer> servers;

            if (System.IO.File.Exists(userServersFile))
            {
                using var reader = System.IO.File.OpenText(userServersFile);
                var rjson = await reader.ReadToEndAsync().ConfigureAwait(false);
                servers = JsonConvert.DeserializeObject<List<TradeServer>>(rjson);
            }
            else
            {
                servers = new List<TradeServer>();
            }

            var dupe = servers.FirstOrDefault(s => s.Name.Equals(server.Name, StringComparison.Ordinal));
            if (dupe != null)
            {
                servers.Remove(dupe);
            }

            servers.Add(server);

            var wjson = JsonConvert.SerializeObject(servers, Formatting.Indented);

            UnicodeEncoding encoding = new UnicodeEncoding();
            char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
            using StreamWriter writer = System.IO.File.CreateText(userServersFile);
            await writer.WriteAsync(chars, 0, chars.Length).ConfigureAwait(false);
        }

        public async Task DeleteTradeServerAsync(TradeServer server)
        {
            if (System.IO.File.Exists(userServersFile))
            {
                List<TradeServer> servers = null;

                using (var reader = System.IO.File.OpenText(userServersFile))
                {
                    var rjson = await reader.ReadToEndAsync().ConfigureAwait(false);
                    servers = JsonConvert.DeserializeObject<List<TradeServer>>(rjson);
                }

                var remove = servers.FirstOrDefault(s => s.Name.Equals(server.Name, StringComparison.Ordinal));
                if (remove != null)
                {
                    servers.Remove(remove);
                    var wjson = JsonConvert.SerializeObject(servers);

                    UnicodeEncoding encoding = new UnicodeEncoding();
                    char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
                    using StreamWriter writer = System.IO.File.CreateText(userServersFile);
                    await writer.WriteAsync(chars, 0, chars.Length).ConfigureAwait(false);
                }
            }
        }

        private static TradeServer GetDemoTradeServer()
        {
            return new TradeServer
            {
                Name = "TradeServer",
                Uri = new Uri("http://localhost:5500"),
                MaxDegreeOfParallelism = 5,
                Enabled = true
            };
        }
    }
}
