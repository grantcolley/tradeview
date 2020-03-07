using DevelopmentInProgress.TradeView.Data;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public class ServerService : IServerService
    {
        private readonly ITradeViewConfigurationServer configurationServer;

        public ServerService(ITradeViewConfigurationServer configurationServer)
        {
            this.configurationServer = configurationServer;
        }

        public async Task<List<Server>> GetServers()
        {
            var result = await configurationServer.GetServersAsync();
            return result.Select(s => s.ToWpfServer()).ToList();
        }

        public async Task<Server> GetServer(string serverName)
        {
            var result = await configurationServer.GetServerAsync(serverName);
            return result.ToWpfServer();
        }

        public Task SaveServer(Server server)
        {
            return configurationServer.SaveServerAsync(server.ToInterfaceServer());
        }

        public Task DeleteServer(Server server)
        {
            return configurationServer.DeleteServerAsync(server.ToInterfaceServer());
        }
    }
}