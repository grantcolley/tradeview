using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.Wpf.Common.Model;
using System.Collections.ObjectModel;

namespace DevelopmentInProgress.Wpf.StrategyManager.Model
{
    public class Strategy : EntityBase
    {
        private StrategyFile targetAssembly;
        private bool isRunEnabled;
        private bool isMonitoEnabled;
        private bool isConnected;
        private bool isStopEnabled;

        public Strategy()
        {
            StrategySubscriptions = new ObservableCollection<StrategySubscription>();
            Dependencies = new ObservableCollection<StrategyFile>();

            IsRunEnabled = true;
            IsMonitoEnabled = true;
            IsConnected = true;
            IsStopEnabled = true;
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

        public bool IsRunEnabled
        {
            get { return isRunEnabled; }
            set
            {
                if(isRunEnabled != value)
                {
                    isRunEnabled = value;
                    OnPropertyChanged("IsRunEnabled");
                }
            }
        }

        public bool IsMonitoEnabled
        {
            get { return isMonitoEnabled; }
            set
            {
                if (isMonitoEnabled != value)
                {
                    isMonitoEnabled = value;
                    OnPropertyChanged("IsMonitoEnabled");
                }
            }
        }

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    OnPropertyChanged("IsConnected");
                }
            }
        }

        public bool IsStopEnabled
        {
            get { return isStopEnabled; }
            set
            {
                if (isStopEnabled != value)
                {
                    isStopEnabled = value;
                    OnPropertyChanged("IsStopEnabled");
                }
            }
        }
    }
}
