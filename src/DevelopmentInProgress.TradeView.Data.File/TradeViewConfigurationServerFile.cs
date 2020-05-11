using DevelopmentInProgress.TradeView.Interface.Server;
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
            userServersFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Environment.MachineName}_Servers.txt");
        }

        public Task<ServerConfiguration> GetServerConfiguration()
        {
            return Task.FromResult(new ServerConfiguration { ObserveServerInterval = 10 });
        }

        public async Task<List<Server>> GetServersAsync()
        {
            if (System.IO.File.Exists(userServersFile))
            {
                using (var reader = System.IO.File.OpenText(userServersFile))
                {
                    var json = await reader.ReadToEndAsync();
                    var Servers = JsonConvert.DeserializeObject<List<Server>>(json);
                    return Servers;
                }
            }

            return new List<Server>
            {
                GetDemoServer()
            };
        }

        public async Task<Server> GetServerAsync(string serverName)
        {
            Server server = null;

            if (System.IO.File.Exists(userServersFile))
            {
                using (var reader = System.IO.File.OpenText(userServersFile))
                {
                    var json = await reader.ReadToEndAsync();
                    var servers = JsonConvert.DeserializeObject<List<Server>>(json);
                    server = servers.FirstOrDefault(s => s.Name.Equals(serverName));
                    return server;
                }
            }

            return GetDemoServer();
        }

        public async Task SaveServerAsync(Server server)
        {
            if (server == null)
            {
                return;
            }

            List<Server> servers;

            if (System.IO.File.Exists(userServersFile))
            {
                using (var reader = System.IO.File.OpenText(userServersFile))
                {
                    var rjson = await reader.ReadToEndAsync();
                    servers = JsonConvert.DeserializeObject<List<Server>>(rjson);
                }
            }
            else
            {
                servers = new List<Server>();
            }

            var dupe = servers.FirstOrDefault(s => s.Name.Equals(server.Name));
            if (dupe != null)
            {
                servers.Remove(dupe);
            }

            servers.Add(server);

            var wjson = JsonConvert.SerializeObject(servers, Formatting.Indented);

            UnicodeEncoding encoding = new UnicodeEncoding();
            char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
            using (StreamWriter writer = System.IO.File.CreateText(userServersFile))
            {
                await writer.WriteAsync(chars, 0, chars.Length);
            }
        }

        public async Task DeleteServerAsync(Server server)
        {
            if (System.IO.File.Exists(userServersFile))
            {
                List<Server> servers = null;

                using (var reader = System.IO.File.OpenText(userServersFile))
                {
                    var rjson = await reader.ReadToEndAsync();
                    servers = JsonConvert.DeserializeObject<List<Server>>(rjson);
                }

                var remove = servers.FirstOrDefault(s => s.Name.Equals(server.Name));
                if (remove != null)
                {
                    servers.Remove(remove);
                    var wjson = JsonConvert.SerializeObject(servers);

                    UnicodeEncoding encoding = new UnicodeEncoding();
                    char[] chars = encoding.GetChars(encoding.GetBytes(wjson));
                    using (StreamWriter writer = System.IO.File.CreateText(userServersFile))
                    {
                        await writer.WriteAsync(chars, 0, chars.Length);
                    }
                }
            }
        }

        private Server GetDemoServer()
        {
            return new Server
            {
                Name = "TradeServer",
                Url = "http://localhost:5500",
                MaxDegreeOfParallelism = 5,
                Enabled = true
            };
        }
    }
}
