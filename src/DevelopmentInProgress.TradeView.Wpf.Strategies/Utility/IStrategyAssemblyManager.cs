using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies.Utility
{
    public interface IStrategyAssemblyManager : IDisposable
    {
        string Id { get; }
        string StrategyDirectory { get; }
        List<string> Files { get; }
        object StrategyDisplayView { get; }
        object StrategyDisplayViewModel { get; }
        void Activate(Strategy strategy, Dispatcher UiDispatcher, ILoggerFacade Logger);
    }
}
