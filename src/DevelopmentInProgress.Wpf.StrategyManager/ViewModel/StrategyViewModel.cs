using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.StrategyManager.Model;
using DevelopmentInProgress.Wpf.StrategyManager.Services;

namespace DevelopmentInProgress.Wpf.StrategyManager.ViewModel
{
    public class StrategyViewModel : DocumentViewModel
    {
        private IStrategyService strategyService;
        private Strategy strategy;

        public StrategyViewModel(ViewModelContext viewModelContext, IStrategyService strategyService)
            : base(viewModelContext)
        {
            this.strategyService = strategyService;
        }

        protected override void OnPublished(object data)
        {
            Strategy = strategyService.GetStrategy(Title);
        }

        public Strategy Strategy
        {
            get { return strategy; }
            set
            {
                if(strategy!= value)
                {
                    strategy = value;
                    OnPropertyChanged("Strategy");
                }
            }
        }
    }
}
