using DevelopmentInProgress.MarketView.Interface.Extensions;
using DevelopmentInProgress.Wpf.Controls.Messaging;
using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.StrategyManager.Model;
using DevelopmentInProgress.Wpf.StrategyManager.Services;
using System;
using System.Collections.Generic;
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
            DeleteStrategyDependencyCommand = new ViewModelCommand(DeleteStrategyDependency);
        }

        public ICommand AddStrategyCommand { get; set; }
        public ICommand DeleteStrategyCommand { get; set; }
        public ICommand AddStrategySubscriptionCommand { get; set; }
        public ICommand DeleteStrategySubscriptionCommand { get; set; }
        public ICommand DeleteStrategyDependencyCommand { get; set; }

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

        public IEnumerable<string> TargetAssembly
        {
            set
            {
                if(SelectedStrategy != null)
                {
                    if(value != null
                        && value.Count() > 0)
                    {
                        SelectedStrategy.TargetAssembly = new StrategyFile { File = value.First() };
                    }
                }
            }
        }

        public IEnumerable<string> Dependencies
        {
            set
            {
                if (SelectedStrategy != null)
                {
                    if (value != null
                        && value.Count() > 0)
                    {
                        foreach (string file in value)
                        {
                            SelectedStrategy.Dependencies.Insert(0, new StrategyFile { File = file });
                        }
                    }
                }
            }
        }

        public string[] Exchanges
        {
            get { return ExchangeExtensions.Exchanges(); }
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
            if(SelectedStrategy == null
                || string.IsNullOrEmpty(param.ToString()))
            {
                return;
            }

            var symbol = param.ToString();

            SelectedStrategy.StrategySubscriptions.Insert(0, new StrategySubscription { Symbol = symbol });
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

        private void DeleteStrategyDependency(object param)
        {
            if (SelectedStrategy == null)
            {
                return;
            }

            var file = param as StrategyFile;
            if (file != null)
            {
                SelectedStrategy.Dependencies.Remove(file);
                strategyService.SaveStrategy(SelectedStrategy);
            }
        }
    }
}