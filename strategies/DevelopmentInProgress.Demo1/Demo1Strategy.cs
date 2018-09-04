using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Demo1
{
    public class Demo1Strategy : ITradeStrategy
    {
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyAccountInfoEvent;
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyNotificationEvent;
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyOrderBookEvent;
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyTradeEvent;

        public async Task<Strategy> RunAsync(Strategy strategy)
        {
            while (true)
            {
                await Task.Delay(100000);
            }

            return strategy;
        }

        public void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs)
        {
            StrategyAccountInfoEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Message = "SubscribeAccountInfo" } });
        }

        public void SubscribeAccountInfoException(Exception exception)
        {
            StrategyAccountInfoEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Message = "SubscribeAccountInfoException" } });
        }

        public void SubscribeAggregateTrades(AggregateTradeEventArgs aggregateTradeEventArgs)
        {
            StrategyTradeEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Message = "SubscribeAggregateTrades" } });
        }

        public void SubscribeAggregateTradesException(Exception exception)
        {
            StrategyTradeEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Message = "SubscribeAggregateTradesException" } });
        }

        public void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs)
        {
            StrategyOrderBookEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Message = "SubscribeOrderBook" } });
        }

        public void SubscribeOrderBookException(Exception exception)
        {
            StrategyOrderBookEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Message = "SubscribeOrderBookException" } });
        }

        public void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs)
        {
            StrategyNotificationEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Message = "SubscribeStatistics" } });
        }

        public void SubscribeStatisticsException(Exception exception)
        {
            StrategyNotificationEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Message = "SubscribeStatisticsException" } });
        }
    }
}
