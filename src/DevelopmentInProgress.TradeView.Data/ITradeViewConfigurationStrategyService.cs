using DevelopmentInProgress.TradeView.Interface.Strategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Data
{
    public interface ITradeViewConfigurationStrategyService
    {
        Task<List<StrategyService>> GetStrategyServicesAsync();
        Task<StrategyService> GetStrategyServiceAsync(string strategyServiceName);
        Task SaveStrategyServiceAsync(StrategyService strategyService);
        Task DeleteStrategyServiceAsync(StrategyService strategyService);
    }
}