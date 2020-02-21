using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Server
{
    public interface IServer
    {
        string Name { get; set; }
        string Url { get; set; }
        int MaxDegreeOfParallelism { get; set; }
        string StartedBy { get; set; }
        string StoppedBy { get; set; }
        DateTime Started { get; set; }
        DateTime Stopped { get; set; }
        List<StrategyManager> Strategies { get; set; }
    }
}