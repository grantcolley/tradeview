using DevelopmentInProgress.TradeView.Data;
using DevelopmentInProgress.TradeView.Wpf.Common.Cache;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public class TradeServerService : ITradeServerService
    {
        private readonly ITradeViewConfigurationServer configurationServer;

        public TradeServerService(ITradeViewConfigurationServer configurationServer)
        {
            this.configurationServer = configurationServer;
        }

        public async Task<List<TradeServer>> GetTradeServers()
        {
            var result = await configurationServer.GetTradeServersAsync().ConfigureAwait(false);
            return result.Select(s => s.ToWpfTradeServer()).ToList();
        }

        public async Task<TradeServer> GetTradeServer(string serverName)
        {
            var result = await configurationServer.GetTradeServerAsync(serverName).ConfigureAwait(false);
            return result.ToWpfTradeServer();
        }

        public async Task SaveTradeServer(TradeServer server)
        {
            await configurationServer.SaveTradeServerAsync(server.ToCoreTradeServer()).ConfigureAwait(false);
        }

        public async Task DeleteTradeServer(TradeServer server)
        {
            await configurationServer.DeleteTradeServerAsync(server.ToCoreTradeServer()).ConfigureAwait(false);
        }
    }
}