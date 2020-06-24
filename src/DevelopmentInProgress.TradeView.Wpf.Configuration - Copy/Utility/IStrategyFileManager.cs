using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.Utility
{
    public interface IStrategyFileManager
    {
        string GetStrategyTypeAsJson(StrategyFile strategyFile);
    }
}
