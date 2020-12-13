using System;
using System.Threading;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public class SubscribeOrderBook : SymbolSubscriptionBase<OrderBookEventArgs>
    {
        public SubscribeOrderBook(string symbol, int limit, IExchangeApi exchangeApi)
            : base(symbol, limit, exchangeApi)
        {
        }

        public override void ExchangeSubscribe(Action<OrderBookEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeApi.SubscribeOrderBook(Symbol, Limit, update, exception, cancellationToken);
        }
    }
}