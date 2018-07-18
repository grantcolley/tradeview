using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.Wpf.Controls.Messaging;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.StrategyManager.Services;
using System;
using System.Collections.ObjectModel;
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
        }

        public ICommand AddStrategyCommand { get; set; }
        public ICommand DeleteStrategyCommand { get; set; }
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

            var strategy = new Strategy { Name = param.ToString() };
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
    }
}