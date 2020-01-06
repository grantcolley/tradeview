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
        private List<Symbol> symbols;
        private bool isActive;

        public StrategyDisplayViewModelBase(Strategy strategy, ITradeHelper tradeHelper, Dispatcher UiDispatcher, ILoggerFacade logger)
            : base(logger)
        {
            TradeHelper = tradeHelper;
            Dispatcher = UiDispatcher;
            Strategy = strategy;
        }

        public Strategy Strategy { get; set; }

        public ITradeHelper TradeHelper { get; private set; }

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

        public virtual Task TradeNotificationsAsync(List<Interface.Strategy.StrategyNotification> tradeNotifications)
        {
            throw new NotImplementedException("StrategyDisplayViewModelBase.TradeNotifications(List<Interface.Strategy.StrategyNotification> tradeNotifications)");
        }

        public virtual Task OrderNotificationsAsync(List<Interface.Strategy.StrategyNotification> orderNotifications)
        {
            throw new NotImplementedException("StrategyDisplayViewModelBase.OrderNotifications(List<Interface.Strategy.StrategyNotification> orderNotifications)");
        }

        public virtual Task CandlestickNotificationsAsync(List<Interface.Strategy.StrategyNotification> candlestickNotifications)
        {
            throw new NotImplementedException("StrategyDisplayViewModelBase.CandlestickNotifications(List<Interface.Strategy.StrategyNotification> candlestickNotifications)");
        }
    }
}