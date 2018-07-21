using DevelopmentInProgress.Wpf.StrategyManager.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.StrategyManager.Services
{
    public interface IStrategyService
    {
        List<Strategy> GetStrategies();
        void SaveStrategy(Strategy strategy);
        void DeleteStrategy(Strategy strategy);
    }
}
