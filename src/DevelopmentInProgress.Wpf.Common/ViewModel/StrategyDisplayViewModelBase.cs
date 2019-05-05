using DevelopmentInProgress.Wpf.Common.Model;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace DevelopmentInProgress.Wpf.Common.ViewModel
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

        public virtual void TradeNotifications(List<MarketView.Interface.Strategy.StrategyNotification> tradeNotifications)
        {
            throw new NotImplementedException("StrategyDisplayViewModelBase.UpdateTrades(IEnumerable<ITrade>)");
        }
    }
}