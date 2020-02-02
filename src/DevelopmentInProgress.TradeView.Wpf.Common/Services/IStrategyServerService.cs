using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public interface IStrategyServerService
    {
        Task<List<StrategyServer>> GetStrategyServers();
        Task<StrategyServer> GetStrategyServer(string strategyServerName);
        Task SaveStrategyServer(StrategyServer strategyServer);
        Task DeleteStrategyServer(StrategyServer strategyServer);
    }
}