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

        public ObservableCollection<StrategySubscription> StrategySubscriptions { get; }
        public ObservableCollection<StrategyFile> DisplayDependencies { get; }
        public ObservableCollection<StrategyFile> Dependencies { get; }
        public StrategyStatus Status { get; set; }
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
                    OnPropertyChanged(nameof(Name));
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
                    OnPropertyChanged(nameof(TargetAssembly));
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
                    OnPropertyChanged(nameof(DisplayAssembly));
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
                    OnPropertyChanged(nameof(Parameters));
                }
            }
        }
    }
}
