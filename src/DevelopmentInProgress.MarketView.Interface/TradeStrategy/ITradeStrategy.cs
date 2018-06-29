using DevelopmentInProgress.MarketView.Interface.Events;
using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Interface.TradeStrategy
{
    public interface ITradeStrategy
    {
        Task<Strategy> RunAsync(Strategy strategy);

        event EventHandler<TradeStrategyNotificationEventArgs> TradeStrategyNotificationEvent;

        void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs);
        void SubscribeStatisticsException(Exception exception);

        void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs);
        void SubscribeOrderBookException(Exception exception);

        void SubscribeAggregateTrades(AggregateTradeEventArgs aggregateTradeEventArgs);
        void SubscribeAggregateTradesException(Exception exception);

        void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs);
        void SubscribeAccountInfoException(Exception exception);
    }
}