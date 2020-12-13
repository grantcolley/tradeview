using System;
using System.Threading;
using DevelopmentInProgress.TradeView.Core.Model;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public class SubscribeCandlesticks : SymbolSubscriptionBase<CandlestickEventArgs>
    {
        public SubscribeCandlesticks(string symbol, int limit, IExchangeApi exchangeApi, CandlestickInterval candlestickInterval)
            : base(symbol, limit, exchangeApi)
        {
            CandlestickInterval = candlestickInterval;
        }

        public CandlestickInterval CandlestickInterval { get; private set; }

        public override void ExchangeSubscribe(Action<CandlestickEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeApi.SubscribeCandlesticks(Symbol, CandlestickInterval, Limit, update, exception, cancellationToken);
        }
    }
}