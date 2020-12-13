using DevelopmentInProgress.TradeView.Core.Interfaces;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public abstract class SymbolSubscriptionBase<T> : SubscriptionBase<T>
    {
        public SymbolSubscriptionBase(string symbol, int limit, IExchangeApi exchangeApi)
            : base(exchangeApi)
        {
            Symbol = symbol;
            Limit = limit;
        }

        public string Symbol { get; private set; }

        public int Limit { get; private set; }
    }
}
