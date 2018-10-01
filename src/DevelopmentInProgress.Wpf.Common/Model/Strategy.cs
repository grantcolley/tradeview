using DevelopmentInProgress.MarketView.Interface.Strategy;
using System.Collections.ObjectModel;

namespace DevelopmentInProgress.Wpf.Common.Model
{
    public class Strategy : EntityBase
    {
        private string name;
        private StrategyFile targetAssembly;

        public Strategy()
        {
            StrategySubscriptions = new ObservableCollection<StrategySubscription>();
            Dependencies = new ObservableCollection<StrategyFile>();
        }

        public StrategyStatus Status { get; set; }
        public ObservableCollection<StrategySubscription> StrategySubscriptions { get; set; }
        public ObservableCollection<StrategyFile> Dependencies { get; set; }
        public string TargetType { get; set; }
        public string Tag { get; set; }
        public string StrategyServerUrl { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

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
