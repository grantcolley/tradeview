using DevelopmentInProgress.TradeView.Data;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public class StrategyServerService : IStrategyServerService
    {
        private readonly ITradeViewConfigurationStrategyServer configurationStrategyServer;

        public StrategyServerService(ITradeViewConfigurationStrategyServer configurationStrategyServer)
        {
            this.configurationStrategyServer = configurationStrategyServer;
        }

        public async Task<List<StrategyServer>> GetStrategyServers()
        {
            var result = await configurationStrategyServer.GetStrategyServersAsync();
            return result.Select(s => s.ToWpfStrategyServer()).ToList();
        }

        public async Task<StrategyServer> GetStrategyServer(string strategyServerName)
        {
            var result = await configurationStrategyServer.GetStrategyServerAsync(strategyServerName);
            return result.ToWpfStrategyServer();
        }

        public Task SaveStrategyServer(StrategyServer strategyServer)
        {
            return configurationStrategyServer.SaveStrategyServerAsync(strategyServer.ToInterfaceStrategyServer());
        }

        public Task DeleteStrategyServer(StrategyServer strategyServer)
        {
            return configurationStrategyServer.DeleteStrategyServerAsync(strategyServer.ToInterfaceStrategyServer());
        }
    }
}