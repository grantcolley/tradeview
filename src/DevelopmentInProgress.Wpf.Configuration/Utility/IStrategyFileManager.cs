using DevelopmentInProgress.Wpf.Common.Model;

namespace DevelopmentInProgress.Wpf.Configuration.Utility
{
    public interface IStrategyFileManager
    {
        string GetStrategyTypeAsJson(StrategyFile strategyFile);
    }
}
