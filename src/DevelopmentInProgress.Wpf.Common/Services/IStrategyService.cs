using DevelopmentInProgress.Wpf.Common.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.Common.Services
{
    public interface IStrategyService
    {
        List<Strategy> GetStrategies();
        Strategy GetStrategy(string strategyName);
        void SaveStrategy(Strategy strategy);
        void DeleteStrategy(Strategy strategy);
    }
}