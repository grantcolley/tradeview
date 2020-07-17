using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Helpers;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DevelopmentInProgress.TradeView.Wpf.Common.ViewModel
{
    public abstract class StrategyDisplayViewModelBase : BaseViewModel
    {
        private bool isActive;

        public StrategyDisplayViewModelBase(Strategy strategy,
            IHelperFactoryContainer iHelperFactoryContainer, 
            Dispatcher UiDispatcher, ILoggerFacade logger)
            : base(logger)
        {
            HelperFactoryContainer = iHelperFactoryContainer;
            Dispatcher = UiDispatcher;
            Strategy = strategy;

            Symbols = new List<Symbol>();
        }

        public event EventHandler<StrategyEventArgs> OnStrategyNotification;

        public List<Symbol> Symbols { get; }

        public Strategy Strategy { get; set; }

        public IHelperFactoryContainer HelperFactoryContainer { get; private set; }

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        public virtual Task TradeNotificationsAsync(List<Core.TradeStrategy.StrategyNotification> tradeNotifications)
        {
            throw new NotImplementedException("StrategyDisplayViewModelBase.TradeNotifications(List<Core.TradeStrategy.StrategyNotification> tradeNotifications)");
        }

        public virtual Task OrderNotificationsAsync(List<Core.TradeStrategy.StrategyNotification> orderNotifications)
        {
            throw new NotImplementedException("StrategyDisplayViewModelBase.OrderNotifications(List<Core.TradeStrategy.StrategyNotification> orderNotifications)");
        }

        public virtual Task CandlestickNotificationsAsync(List<Core.TradeStrategy.StrategyNotification> candlestickNotifications)
        {
            throw new NotImplementedException("StrategyDisplayViewModelBase.CandlestickNotifications(List<Core.TradeStrategy.StrategyNotification> candlestickNotifications)");
        }

        protected void OnException(string message, Exception exception)
        {
            var onStrategyNotification = OnStrategyNotification;
            onStrategyNotification?.Invoke(this, new StrategyEventArgs { Value = Strategy, Message = message, Exception = exception });
        }

        protected void OnNotify(string message)
        {
            var onStrategyNotification = OnStrategyNotification;
            onStrategyNotification?.Invoke(this, new StrategyEventArgs { Value = Strategy, Message = message });
        }
    }
}