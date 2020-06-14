using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using System.Collections.ObjectModel;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class Strategy : EntityBase
    {
        private string name;
        private StrategyFile targetAssembly;
        private StrategyFile displayAssembly;
        private string parameters;

        public Strategy()
        {
            StrategySubscriptions = new ObservableCollection<StrategySubscription>();
            Dependencies = new ObservableCollection<StrategyFile>();
            DisplayDependencies = new ObservableCollection<StrategyFile>();
        }

        public StrategyStatus Status { get; set; }
        public ObservableCollection<StrategySubscription> StrategySubscriptions { get; set; }
        public ObservableCollection<StrategyFile> Dependencies { get; set; }
        public ObservableCollection<StrategyFile> DisplayDependencies { get; set; }
        public string TargetType { get; set; }
        public string DisplayViewType { get; set; }
        public string DisplayViewModelType { get; set; }
        public int TradesChartDisplayCount { get; set; }
        public int TradesDisplayCount { get; set; }
        public int OrderBookChartDisplayCount { get; set; }
        public int OrderBookDisplayCount { get; set; }

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

        public StrategyFile DisplayAssembly
        {
            get { return displayAssembly; }
            set
            {
                if (displayAssembly != value)
                {
                    displayAssembly = value;
                    OnPropertyChanged("DisplayAssembly");
                }
            }
        }

        public string Parameters
        {
            get { return parameters; }
            set
            {
                if (parameters != value)
                {
                    parameters = value;
                    OnPropertyChanged("Parameters");
                }
            }
        }
    }
}
