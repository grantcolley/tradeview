using DevelopmentInProgress.TradeView.Core.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Command;
using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Configuration.Utility;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "General exceptions are reported to subscribers.")]
    public class StrategyViewModel : BaseViewModel
    {
        private readonly IStrategyService strategyService;
        private readonly IStrategyFileManager strategyFileManager;
        private Strategy strategy;
        bool disposed = false;

        public StrategyViewModel(Strategy strategy, IStrategyService strategyService, IStrategyFileManager strategyFileManager, ILoggerFacade logger)
            : base(logger)
        {
            this.strategy = strategy;
            this.strategyService = strategyService;
            this.strategyFileManager = strategyFileManager;

            AddStrategySubscriptionCommand = new ViewModelCommand(AddStrategySubscription);
            DeleteStrategySubscriptionCommand = new ViewModelCommand(DeleteStrategySubscription);
            DeleteStrategyDependencyCommand = new ViewModelCommand(DeleteStrategyDependency);
            AddParameterSchemaCommand = new ViewModelCommand(AddParameterSchema);
            DisplayDependenciesCommand = new ViewModelCommand(DisplayDependencies);
            DisplayAssemblyCommand = new ViewModelCommand(DisplayAssembly);
            TargetAssemblyCommand = new ViewModelCommand(TargetAssembly);
            DependenciesCommand = new ViewModelCommand(Dependencies);

            OnPropertyChanged(string.Empty);
        }

        public event EventHandler<StrategyEventArgs> OnStrategyNotification;

        public ICommand AddStrategySubscriptionCommand { get; set; }
        public ICommand DeleteStrategySubscriptionCommand { get; set; }
        public ICommand DeleteStrategyDependencyCommand { get; set; }
        public ICommand AddParameterSchemaCommand { get; set; }
        public ICommand DisplayDependenciesCommand { get; set; }
        public ICommand DisplayAssemblyCommand { get; set; }
        public ICommand TargetAssemblyCommand { get; set; }
        public ICommand DependenciesCommand { get; set; }

        public Strategy Strategy
        {
            get { return strategy; }
            set
            {
                if (strategy != value)
                {
                    strategy = value;
                    OnPropertyChanged(nameof(Strategy));
                }
            }
        }

        public static List<string> Exchanges
        {
            get { return ExchangeExtensions.Exchanges().ToList(); }
        }

        public static List<string> CandlestickIntervals
        {
            get { return CandlestickIntervalExtensions.GetCandlestickIntervalNames().ToList(); }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // dispose stuff...
            }

            disposed = true;
        }

        private void DisplayAssembly(object arg)
        {
            IEnumerable<string> files = arg as IEnumerable<string>;

            if (files == null)
            {
                return;
            }

            if (Strategy != null)
            {
                if (files.Any())
                {
                    var file = files.First();
                    Strategy.DisplayAssembly = new StrategyFile { File = file };

                    if (!Strategy.DisplayDependencies.Any(d => d.File.Equals(file, StringComparison.Ordinal)))
                    {
                        Strategy.DisplayDependencies.Insert(0, new StrategyFile { File = file, FileType = StrategyFileType.DisplayFile });
                    }
                }
            }
        }

        private void DisplayDependencies(object arg)
        {
            IEnumerable<string> files = arg as IEnumerable<string>;

            if (files == null)
            {
                return;
            }

            if (Strategy != null)
            {
                if (files.Any())
                {
                    foreach (string file in files)
                    {
                        Strategy.DisplayDependencies.Insert(0, new StrategyFile { File = file, FileType = StrategyFileType.DisplayFile });
                    }
                }
            }
        }

        private void TargetAssembly(object arg)
        {
            IEnumerable<string> files = arg as IEnumerable<string>;

            if (files == null)
            {
                return;
            }

            if (Strategy != null)
            {
                if (files.Any())
                {
                    var file = files.First();
                    Strategy.TargetAssembly = new StrategyFile { File = file };

                    if (!Strategy.Dependencies.Any(d => d.File.Equals(file, StringComparison.Ordinal)))
                    {
                        Strategy.Dependencies.Insert(0, new StrategyFile { File = file, FileType = StrategyFileType.StrategyFile });
                    }
                }
            }
        }

        private void Dependencies(object arg)
        {
            IEnumerable<string> files = arg as IEnumerable<string>;

            if (files == null)
            {
                return;
            }

            if (Strategy != null)
            {
                if (files.Any())
                {
                    foreach (string file in files)
                    {
                        Strategy.Dependencies.Insert(0, new StrategyFile { File = file, FileType = StrategyFileType.StrategyFile });
                    }
                }
            }
        }

        private async void AddStrategySubscription(object param)
        {
            if (Strategy == null
                || string.IsNullOrEmpty(param.ToString()))
            {
                return;
            }

            var symbol = param.ToString();

            try
            {
                Strategy.StrategySubscriptions.Insert(0, new StrategySubscription { Symbol = symbol });
                await strategyService.SaveStrategy(Strategy).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                OnStrategyException(ex);
            }
        }

        private async void DeleteStrategySubscription(object param)
        {
            if (Strategy == null)
            {
                return;
            }

            var subscription = param as StrategySubscription;
            if (subscription != null)
            {
                try
                {
                    Strategy.StrategySubscriptions.Remove(subscription);
                    await strategyService.SaveStrategy(Strategy).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    OnStrategyException(ex);
                }
            }
        }

        private async void DeleteStrategyDependency(object param)
        {
            if (Strategy == null)
            {
                return;
            }

            var file = param as StrategyFile;
            if (file != null)
            {
                try
                {
                    if(file.FileType.Equals(StrategyFileType.StrategyFile))
                    {
                        Strategy.Dependencies.Remove(file);
                    }
                    else if(file.FileType.Equals(StrategyFileType.DisplayFile))
                    {
                        Strategy.DisplayDependencies.Remove(file);
                    }

                    await strategyService.SaveStrategy(Strategy).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    OnStrategyException(ex);
                }
            }
        }

        private async void AddParameterSchema(object param)
        {
            if(Strategy == null)
            {
                return;
            }

            if(Strategy.TargetAssembly == null
                || string.IsNullOrWhiteSpace(Strategy.TargetAssembly.File))
            {
                OnNotification($"{Strategy.Name}'s {nameof(Strategy.TargetAssembly)} must be specified.");
                return;
            }

            try
            {
                var strategyTypeJson = strategyFileManager.GetStrategyTypeAsJson(strategy.TargetAssembly);

                if(string.IsNullOrWhiteSpace(strategyTypeJson))
                {
                    return;
                }

                Strategy.Parameters = strategyTypeJson;

                await strategyService.SaveStrategy(Strategy).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                OnStrategyException(ex);
            }
        }

        private void OnStrategyException(Exception exception)
        {
            OnNotification(exception.Message, exception);
        }

        private void OnNotification(string message)
        {
            OnNotification(message, null);
        }

        private void OnNotification(string message, Exception exception)
        {
            var onStrategyNotification = OnStrategyNotification;
            onStrategyNotification?.Invoke(this, new StrategyEventArgs { Value = Strategy, Message = message, Exception = exception });
        }
    }
}
