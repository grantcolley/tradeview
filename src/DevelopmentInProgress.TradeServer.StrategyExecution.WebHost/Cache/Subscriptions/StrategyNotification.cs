using System;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public class StrategyNotification<T>
    {
        public Action<T> Update { get; set; }
        public Action<Exception> Exception { get; set; }
    }
}