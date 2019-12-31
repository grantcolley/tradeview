using DevelopmentInProgress.TradeView.Wpf.Controls.Messaging;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Reactive.Linq;
using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using System.Collections.Generic;
using DevelopmentInProgress.TradeView.Wpf.Configuration.Utility;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel
{
    public class StrategyManagerViewModel : DocumentViewModel
    {
        private IStrategyService strategyService;
        private IStrategyFileManager strategyFileManager;
        private ObservableCollection<Strategy> strategies;
        private StrategyViewModel selectedStrategyViewModel;
        private Strategy selectedStrategy;
        private Dictionary<string, IDisposable> strategyObservableSubscriptions;
        private bool isLoading;
        private bool disposed;

        public StrategyManagerViewModel(ViewModelContext viewModelContext, IStrategyService strategyService, IStrategyFileManager strategyFileManager)
            : base(viewModelContext)
        {
            this.strategyService = strategyService;
            this.strategyFileManager = strategyFileManager;

            AddStrategyCommand = new ViewModelCommand(AddStrategy);
            DeleteStrategyCommand = new ViewModelCommand(DeleteStrategy);
            CloseCommand = new ViewModelCommand(Close);

            SelectedStrategyViewModels = new ObservableCollection<StrategyViewModel>();
            strategyObservableSubscriptions = new Dictionary<string, IDisposable>();
        }

        public ICommand AddStrategyCommand { get; set; }
        public ICommand DeleteStrategyCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public ObservableCollection<Strategy> Strategies 
        {
            get { return strategies; } 
            set
            {
                if(strategies != value)
                {
                    strategies = value;
                    OnPropertyChanged("Strategies");
                }
            }
        }

        public ObservableCollection<StrategyViewModel> SelectedStrategyViewModels { get; set; }

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged("IsLoading");
                }
            }
        }

        public Strategy SelectedStrategy
        {
            get { return selectedStrategy; }
            set
            {
                if (selectedStrategy != value)
                {
                    selectedStrategy = value;

                    if (selectedStrategy != null)
                    {
                        var strategyViewModel = SelectedStrategyViewModels.FirstOrDefault(s => s.Strategy.Name.Equals(selectedStrategy.Name));

                        if (strategyViewModel == null)
                        {
                            strategyViewModel = new StrategyViewModel(selectedStrategy, strategyService, strategyFileManager, Logger);
                            ObserveStrategy(strategyViewModel);
                            SelectedStrategyViewModels.Add(strategyViewModel);
                            SelectedStrategyViewModel = strategyViewModel;
                        }
                        else
                        {
                            SelectedStrategyViewModel = strategyViewModel;
                        }
                    }

                    OnPropertyChanged("SelectedStrategy");
                }
            }
        }

        public StrategyViewModel SelectedStrategyViewModel
        {
            get { return selectedStrategyViewModel; }
            set
            {
                if (selectedStrategyViewModel != value)
                {
                    selectedStrategyViewModel = value;
                    OnPropertyChanged("SelectedStrategyViewModel");
                }
            }
        }

        public void Close(object param)
        {
            var strategy = param as StrategyViewModel;
            if (strategy != null)
            {
                strategy.Dispose();

                IDisposable subscription;
                if (strategyObservableSubscriptions.TryGetValue(strategy.Strategy.Name, out subscription))
                {
                    subscription.Dispose();
                }

                strategyObservableSubscriptions.Remove(strategy.Strategy.Name);
                
                SelectedStrategyViewModels.Remove(strategy);
            }
        }

        protected async override void OnPublished(object data)
        {
            base.OnPublished(data);

            try
            {
                IsLoading = true;

                var strategies = await strategyService.GetStrategies();

                Strategies = new ObservableCollection<Strategy>(strategies);
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected override void OnDisposing()
        {
            if(disposed)
            {
                return;           
            }

            foreach (var subscription in strategyObservableSubscriptions.Values)
            {
                subscription.Dispose();
            }

            foreach (var strategyViewModel in SelectedStrategyViewModels)
            {
                strategyViewModel.Dispose();
            }

            disposed = true;
        }

        protected async override void SaveDocument()
        {
            try
            {
                IsLoading = true;

                foreach (var strategyViewModel in SelectedStrategyViewModels)
                {
                    await strategyService.SaveStrategy(strategyViewModel.Strategy);
                }
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void AddStrategy(object param)
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

            try
            {
                IsLoading = true;

                var strategy = new Strategy { Name = strategyName };
                await strategyService.SaveStrategy(strategy);
                Strategies.Add(strategy);
                Module.AddStrategy(strategy.Name);
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void DeleteStrategy(object param)
        {
            var strategy = param as Strategy;
            if (strategy == null)
            {
                return;
            }

            var result = Dialog.ShowMessage(new MessageBoxSettings
            {
                Title = "Delete Strategy",
                Text = $"Are you sure you want to delete {strategy.Name}?",
                MessageType = MessageType.Question,
                MessageBoxButtons = MessageBoxButtons.OkCancel
            });

            if (result.Equals(MessageBoxResult.Cancel))
            {
                return;
            }

            var strategyViewModel = SelectedStrategyViewModels.FirstOrDefault(s => s.Strategy.Name.Equals(strategy.Name));
            if(strategyViewModel != null)
            {
                Close(strategyViewModel);
            }

            try
            {
                IsLoading = true;

                await strategyService.DeleteStrategy(strategy);
                Strategies.Remove(strategy);
                Module.RemoveStrategy(strategy.Name);
            }
            catch (Exception ex)
            {
                ShowMessage(new Message { MessageType = MessageType.Error, Text = ex.Message });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ObserveStrategy(StrategyViewModel strategy)
        {
            var strateyObservable = Observable.FromEventPattern<StrategyEventArgs>(
                eventHandler => strategy.OnStrategyNotification += eventHandler,
                eventHandler => strategy.OnStrategyNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            var strateyObservableSubscription = strateyObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    ShowMessage(new Message { MessageType = MessageType.Error, Text = args.Exception.ToString() });
                }
            });

            strategyObservableSubscriptions.Add(strategy.Strategy.Name, strateyObservableSubscription);
        }
    }
}