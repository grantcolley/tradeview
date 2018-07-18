using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.StrategyManager.Services;

namespace DevelopmentInProgress.Wpf.StrategyManager.ViewModel
{
    public class StrategyViewModel : DocumentViewModel
    {
        private IStrategyService strategyService;

        public StrategyViewModel(ViewModelContext viewModelContext, IStrategyService strategyService)
            : base(viewModelContext)
        {
            this.strategyService = strategyService;
        }
    }
}
