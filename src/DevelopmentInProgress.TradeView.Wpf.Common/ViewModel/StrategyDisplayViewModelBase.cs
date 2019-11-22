using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace DevelopmentInProgress.TradeView.Wpf.Common.ViewModel
{
    public abstract class StrategyDisplayViewModelBase : BaseViewModel
    {
        private List<Symbol> symbols;
        private bool isActive;

        public StrategyDisplayViewModelBase(Strategy strategy, Dispatcher UiDispatcher, ILoggerFacade logger)
            : base(logger)
        {
            Dispatcher = UiDispatcher;
            Strategy = strategy;
        }

        public Strategy Strategy { get; set; }

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    OnPropertyChanged("IsActive");
                }
            }
        }

        public List<Symbol> Symbols
        {
            get { return symbols; }
            set
            {
                if (symbols != value)
                {
                    symbols = value;
                    OnPropertyChanged("Symbols");
                }
            }
        }

        public virtual void TradeNotifications(List<Interface.Strategy.StrategyNotification> tradeNotifications)
        {
            throw new NotImplementedException("StrategyDisplayViewModelBase.TradeNotifications(List<Interface.Strategy.StrategyNotification> tradeNotifications)");
        }

        public virtual void OrderNotifications(List<Interface.Strategy.StrategyNotification> orderNotifications)
        {
            throw new NotImplementedException("StrategyDisplayViewModelBase.OrderNotifications(List<Interface.Strategy.StrategyNotification> orderNotifications)");
        }

        public virtual void CandlestickNotifications(List<Interface.Strategy.StrategyNotification> candlestickNotifications)
        {
            throw new NotImplementedException("StrategyDisplayViewModelBase.CandlestickNotifications(List<Interface.Strategy.StrategyNotification> candlestickNotifications)");
        }
    }
}