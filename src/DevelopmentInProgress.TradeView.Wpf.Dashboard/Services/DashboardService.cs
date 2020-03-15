using DevelopmentInProgress.TradeView.Data;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ITradeViewConfigurationServer configurationServer;

        public DashboardService(ITradeViewConfigurationServer configurationServer)
        {
            this.configurationServer = configurationServer;
        }

        public async Task<List<ServerMonitor>> GetServers()
        {
            var result = await configurationServer.GetServersAsync();
           return result.Select(s => s.ToServerMonitor()).ToList();
        }
    }
}
