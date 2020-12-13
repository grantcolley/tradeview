using System;
using System.Threading;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Model;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions
{
    public class SubscribeAccountInfo : SubscriptionBase<AccountInfoEventArgs>
    {
        public SubscribeAccountInfo(IExchangeApi exchangeApi)
            : base(exchangeApi)
        {
            User = new User();
        }

        public User User { get; private set; }

        public override void ExchangeSubscribe(Action<AccountInfoEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeApi.SubscribeAccountInfo(User, update, exception, cancellationToken);
        }
    }
}