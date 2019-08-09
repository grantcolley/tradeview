using DevelopmentInProgress.Wpf.Common.Model;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace DevelopmentInProgress.Wpf.Configuration.Utility
{
    public interface IStrategyAssemblyManager : IDisposable
    {
        string Id { get; }
        string StrategyDirectory { get; }
        List<string> Files { get; }
        object StrategyDisplayView { get; }
        object StrategyDisplayViewModel { get; }
        void Activate(Strategy strategy, Dispatcher UiDispatcher, ILoggerFacade Logger);
        string GetStrategyTypeAsJson(StrategyFile strategyFile);
    }
}
