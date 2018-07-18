using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.StrategyManager.Services
{
    public interface IStrategyService
    {
        IList<Strategy> GetStrategies();
        void SaveStrategy(Strategy strategy);
        void DeleteStrategy(Strategy strategy);
    }
}
