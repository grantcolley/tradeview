using System;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public interface ISubscriptionBase<T> : IDisposable
    {
        bool HasSubscriptions { get; }
        int Subscriptions { get; }
        void Subscribe(string strategyName, StrategyNotification<T> strategyNotification);
        void Unsubscribe(string strategyName, Action<Exception> exception);
    }
}
