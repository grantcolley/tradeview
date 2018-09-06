using DevelopmentInProgress.MarketView.Interface.Events;
using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Interface.TradeStrategy
{
    public interface ITradeStrategy
    {
        Task<Strategy> RunAsync(Strategy strategy);

        event EventHandler<TradeStrategyNotificationEventArgs> StrategyAccountInfoEvent;
        
        event EventHandler<TradeStrategyNotificationEventArgs> StrategyNotificationEvent;

        event EventHandler<TradeStrategyNotificationEventArgs> StrategyOrderBookEvent;

        event EventHandler<TradeStrategyNotificationEventArgs> StrategyTradeEvent;

        void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs);

        void SubscribeStatisticsException(Exception exception);

        void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs);

        void SubscribeOrderBookException(Exception exception);

        void SubscribeTrades(AggregateTradeEventArgs aggregateTradeEventArgs);

        void SubscribeTradesException(Exception exception);

        void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs);

        void SubscribeAccountInfoException(Exception exception);
    }
}