using DevelopmentInProgress.TradeView.Interface.Strategy;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Data
{
    public interface ITradeViewStrategyPerformance
    {
        Task<StrategyPerformance> GetStrategyPerformance(string strategyName);
        Task SaveStrategyPerformance(StrategyPerformance strategyPerformance);
    }
}
