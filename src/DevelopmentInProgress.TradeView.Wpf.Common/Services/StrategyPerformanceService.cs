using DevelopmentInProgress.TradeView.Data;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public class StrategyPerformanceService : IStrategyPerformanceService
    {
        private readonly ITradeViewStrategyPerformance strategyPerformanceData;

        public StrategyPerformanceService(ITradeViewStrategyPerformance strategyPerformance)
        {
            this.strategyPerformanceData = strategyPerformance;
        }

        public Task<StrategyPerformance> GetStrategyPerformance(string strategyName)
        {
            return strategyPerformanceData.GetStrategyPerformance(strategyName);
        }

        public Task SaveStrategyPerformance(StrategyPerformance strategyPerformance)
        {
            return strategyPerformanceData.SaveStrategyPerformance(strategyPerformance);
        }
    }
}
