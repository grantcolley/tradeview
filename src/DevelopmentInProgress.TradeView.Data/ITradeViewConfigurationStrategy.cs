using DevelopmentInProgress.TradeView.Interface.Strategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Data
{
    public interface ITradeViewConfigurationStrategy
    {
        Task<List<StrategyConfig>> GetStrategiesAsync();
        Task<StrategyConfig> GetStrategyAsync(string strategyName);
        Task SaveStrategyAsync(StrategyConfig strategyConfig);
        Task DeleteStrategyAsync(StrategyConfig strategyConfig);
    }
}
