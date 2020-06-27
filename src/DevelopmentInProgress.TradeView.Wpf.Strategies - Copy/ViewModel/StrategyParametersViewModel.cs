using DevelopmentInProgress.TradeView.Wpf.Common.Command;
using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using Prism.Logging;
using System;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies.ViewModel
{
    public class StrategyParametersViewModel : BaseViewModel
    {
        private bool disposed;
        private Strategy strategy;
        private bool canPushParameters;

        public StrategyParametersViewModel(ILoggerFacade logger)
            : base(logger)
        {
            canPushParameters = false;
            PushStrategyParametersCommand = new ViewModelCommand(PushStrategyParameters);
        }

        public event EventHandler<StrategyEventArgs> OnStrategyParametersNotification;

        public ICommand PushStrategyParametersCommand { get; set; }

        public bool CanPushParameters
        {
            get { return canPushParameters; }
            set
            {
                if(canPushParameters != value)
                {
                    canPushParameters = value;
                    OnPropertyChanged("CanPushParameters");
                }
            }
        }

        public Strategy Strategy
        {
            get { return strategy; }
            set
            {
                if (strategy != value)
                {
                    strategy = value;
                    OnPropertyChanged("Strategy");
                }
            }
        }

        public override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {

            }

            disposed = true;
        }

        private void PushStrategyParameters(object parameters)
        {
            StrategyParameterNotification();
        }

        private void OnException(string message, Exception exception)
        {
            var onStrategyParametersNotification = OnStrategyParametersNotification;
            onStrategyParametersNotification?.Invoke(this, new StrategyEventArgs { Message = message, Exception = exception });
        }

        private void StrategyParameterNotification()
        {
            var onStrategyParametersNotification = OnStrategyParametersNotification;
            onStrategyParametersNotification?.Invoke(this, new StrategyEventArgs { Value = Strategy });
        }
    }
}
