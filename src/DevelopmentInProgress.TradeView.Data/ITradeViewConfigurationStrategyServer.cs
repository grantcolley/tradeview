using DevelopmentInProgress.TradeView.Interface.Strategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Data
{
    public interface ITradeViewConfigurationStrategyServer
    {
        Task<List<StrategyServer>> GetStrategyServersAsync();
        Task<StrategyServer> GetStrategyServerAsync(string strategyServerName);
        Task SaveStrategyServerAsync(StrategyServer strategyServer);
        Task DeleteStrategyServerAsync(StrategyServer strategyServer);
    }
}