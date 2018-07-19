using DevelopmentInProgress.Wpf.StrategyManager.Model;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.StrategyManager.Services
{
    public class StrategyService : IStrategyService
    {
        public IList<Strategy> GetStrategies()
        {
            return new List<Strategy>();
        }

        public void SaveStrategy(Strategy strategy)
        {
            throw new NotImplementedException();
        }

        public void DeleteStrategy(Strategy strategy)
        {
            throw new NotImplementedException();
        }
    }
}