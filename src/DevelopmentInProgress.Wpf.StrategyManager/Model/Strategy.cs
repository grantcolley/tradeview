using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.Wpf.Common.Model;
using System.Collections.ObjectModel;

namespace DevelopmentInProgress.Wpf.StrategyManager.Model
{
    public class Strategy : EntityBase
    {
        private StrategyFile targetAssembly;

        public Strategy()
        {
            StrategySubscriptions = new ObservableCollection<StrategySubscription>();
            Dependencies = new ObservableCollection<StrategyFile>();
        }

        public string Name { get; set; }
        public StrategyStatus Status { get; set; }
        public ObservableCollection<StrategySubscription> StrategySubscriptions { get; set; }
        public ObservableCollection<StrategyFile> Dependencies { get; set; }
        public string TargetType { get; set; }
        public string Tag { get; set; }

        public StrategyFile TargetAssembly
        {
            get { return targetAssembly; }
            set
            {
                if(targetAssembly != value)
                {
                    targetAssembly = value;
                    OnPropertyChanged("TargetAssembly");
                }
            }
        }
    }
}
