using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Strategies.Utility;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies.Events
{
    public class StrategyDisplayEventArgs : BaseEventArgs<Strategy>
    {
        public IStrategyAssemblyManager StrategyAssemblyManager { get; set; }
    }
}
