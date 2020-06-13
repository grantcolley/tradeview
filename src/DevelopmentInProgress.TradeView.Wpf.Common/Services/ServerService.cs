using DevelopmentInProgress.TradeView.Data;
using DevelopmentInProgress.TradeView.Wpf.Common.Cache;
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
        private readonly IServerMonitorCache serverMonitorCache;

        public ServerService(ITradeViewConfigurationServer configurationServer, IServerMonitorCache serverMonitorCache)
        {
            this.configurationServer = configurationServer;
            this.serverMonitorCache = serverMonitorCache;
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

        public async Task SaveServer(Server server)
        {
            await configurationServer.SaveServerAsync(server.ToCoreServer());
            await serverMonitorCache.RefreshServerMonitorsAsync();
        }

        public async Task DeleteServer(Server server)
        {
            await configurationServer.DeleteServerAsync(server.ToCoreServer());
            await serverMonitorCache.RefreshServerMonitorsAsync();
        }
    }
}