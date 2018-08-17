using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Demo1
{
    public class Demo1Strategy : ITradeStrategy
    {
        public event EventHandler<TradeStrategyNotificationEventArgs> TradeStrategyNotificationEvent;

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
            throw new NotImplementedException();
        }

        public void SubscribeAccountInfoException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAggregateTrades(AggregateTradeEventArgs aggregateTradeEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAggregateTradesException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeOrderBookException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeStatisticsException(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
