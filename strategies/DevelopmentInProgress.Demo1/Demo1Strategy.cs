using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Demo1
{
    public class Demo1Strategy : ITradeStrategy
    {
        private Strategy strategy;

        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyAccountInfoEvent;
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyNotificationEvent;
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyOrderBookEvent;
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyTradeEvent;
               
        public async Task<Strategy> RunAsync(Strategy strategy)
        {
            this.strategy = strategy;

            while (true)
            {
                await Task.Delay(100000);
            }

            return this.strategy;
        }

        public void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs)
        {
            StrategyTradeEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = strategy.Name, Message = "SubscribeAggregateTrades" } });
        }

        public void SubscribeAccountInfoException(Exception exception)
        {
            StrategyAccountInfoEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = strategy.Name, Message = "SubscribeAccountInfoException" } });
        }

        public void SubscribeTrades(AggregateTradeEventArgs aggregateTradeEventArgs)
        {
            var strategyNotification = new StrategyNotification { Name = strategy.Name, NotificationLevel = NotificationLevel.Trade };
            string message;

            try
            {
                message = JsonConvert.SerializeObject(aggregateTradeEventArgs.AggregateTrades);
            }
            catch(Exception ex)
            {
                message = JsonConvert.SerializeObject(ex);
            }

            strategyNotification.Message = message;
            StrategyTradeEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        public void SubscribeTradesException(Exception exception)
        {
            var message = JsonConvert.SerializeObject(exception);

            var strategyNotification = new StrategyNotification { Name = strategy.Name, Message = message, NotificationLevel = NotificationLevel.TradeError };

            StrategyTradeEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        public void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs)
        {
            StrategyOrderBookEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = strategy.Name, Message = "SubscribeOrderBook" } });
        }

        public void SubscribeOrderBookException(Exception exception)
        {
            StrategyOrderBookEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = strategy.Name, Message = "SubscribeOrderBookException" } });
        }

        public void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs)
        {
            StrategyNotificationEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = strategy.Name, Message = "SubscribeStatistics" } });
        }

        public void SubscribeStatisticsException(Exception exception)
        {
            StrategyNotificationEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = strategy.Name, Message = "SubscribeStatisticsException" } });
        }
    }
}
