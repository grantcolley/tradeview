using DevelopmentInProgress.Wpf.Controls.Messaging;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.StrategyManager.Model;
using DevelopmentInProgress.Wpf.StrategyManager.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DevelopmentInProgress.Wpf.StrategyManager.ViewModel
{
    public class StrategyManagerViewModel : DocumentViewModel
    {
        private Strategy selectedStrategy;
        private IStrategyService strategyService;

        public StrategyManagerViewModel(ViewModelContext viewModelContext, IStrategyService strategyService)
            : base(viewModelContext)
        {
            this.strategyService = strategyService;

            AddStrategyCommand = new ViewModelCommand(AddStrategy);
            DeleteStrategyCommand = new ViewModelCommand(DeleteStrategy);
            AddStrategySubscriptionCommand = new ViewModelCommand(AddStrategySubscription);
            DeleteStrategySubscriptionCommand = new ViewModelCommand(DeleteStrategySubscription);
        }

        public ICommand AddStrategyCommand { get; set; }
        public ICommand DeleteStrategyCommand { get; set; }
        public ICommand AddStrategySubscriptionCommand { get; set; }
        public ICommand DeleteStrategySubscriptionCommand { get; set; }
        public ObservableCollection<Strategy> Strategies { get; set; }

        public Strategy SelectedStrategy
        {
            get { return selectedStrategy; }
            set
            {
                if (selectedStrategy != value)
                {
                    selectedStrategy = value;
                    OnPropertyChanged("SelectedStrategy");
                }
            }
        }

        protected override void OnPublished(object data)
        {
            base.OnPublished(data);

            var strategies = strategyService.GetStrategies();
            Strategies = new ObservableCollection<Strategy>(strategies);
        }

        protected override void SaveDocument()
        {
            if (SelectedStrategy != null)
            {
                try
                {
                    strategyService.SaveStrategy(SelectedStrategy);
                }
                catch (Exception ex)
                {
                    ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
                }
            }
        }

        private void AddStrategy(object param)
        {
            if (param == null
                || string.IsNullOrEmpty(param.ToString()))
            {
                return;
            }

            var strategyName = param.ToString();

            if (Strategies.Any(s => s.Name.Equals(strategyName)))
            {
                ShowMessage(new Message { MessageType = MessageType.Info, Text = $"A strategy with the name {strategyName} already exists." });
                return;
            }

            var strategy = new Strategy { Name = strategyName };
            strategyService.SaveStrategy(strategy);
            Strategies.Add(strategy);
            Module.AddStrategy(strategy.Name);
        }

        private void DeleteStrategy(object param)
        {
            var strategy = param as Strategy;
            if (strategy == null)
            {
                return;
            }

            strategyService.DeleteStrategy(strategy);
            Strategies.Remove(strategy);
            Module.RemoveStrategy(strategy.Name);
        }

        private void AddStrategySubscription(object param)
        {
            if(SelectedStrategy == null)
            {
                return;
            }

            SelectedStrategy.StrategySubscriptions.Insert(0, new StrategySubscription());
            strategyService.SaveStrategy(SelectedStrategy);
        }

        private void DeleteStrategySubscription(object param)
        {
            if (SelectedStrategy == null)
            {
                return;
            }

            var subscription = param as StrategySubscription;
            if (subscription != null)
            {
                SelectedStrategy.StrategySubscriptions.Remove(subscription);
                strategyService.SaveStrategy(SelectedStrategy);
            }
        }
    }
}