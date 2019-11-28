using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public interface IStrategyPerformanceService
    {
        Task<Interface.Strategy.StrategyPerformance> GetStrategyPerformance(string strategyName);
        Task SaveStrategyPerformance(Interface.Strategy.StrategyPerformance strategyPerformance);
    }
}
