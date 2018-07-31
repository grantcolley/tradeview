using DevelopmentInProgress.Wpf.StrategyManager.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.StrategyManager.Services
{
    public interface IStrategyService
    {
        List<Strategy> GetStrategies();
        Strategy GetStrategy(string strategyName);
        void SaveStrategy(Strategy strategy);
        void DeleteStrategy(Strategy strategy);
    }
}