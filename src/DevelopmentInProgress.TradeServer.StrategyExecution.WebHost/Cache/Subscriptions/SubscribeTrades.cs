using System;
using System.Threading;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public class SubscribeTrades : SymbolSubscriptionBase<TradeEventArgs>
    {
        public SubscribeTrades(string symbol, int limit, IExchangeApi exchangeApi)
            : base(symbol, limit, exchangeApi)
        {
        }

        public override void ExchangeSubscribe(Action<TradeEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeApi.SubscribeTrades(Symbol, Limit, update, exception, cancellationToken);
        }
    }
}