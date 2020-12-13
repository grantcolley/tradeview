using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestTradeExceptionStrategy : ITradeStrategy
    {
        public event EventHandler<StrategyNotificationEventArgs> StrategyAccountInfoEvent { add { } remove { } }
        public event EventHandler<StrategyNotificationEventArgs> StrategyCustomNotificationEvent { add { } remove { } }
        public event EventHandler<StrategyNotificationEventArgs> StrategyNotificationEvent { add { } remove { } }
        public event EventHandler<StrategyNotificationEventArgs> StrategyOrderBookEvent { add { } remove { } }
        public event EventHandler<StrategyNotificationEventArgs> StrategyTradeEvent { add { } remove { } }
        public event EventHandler<StrategyNotificationEventArgs> StrategyCandlesticksEvent { add { } remove { } }
        public event EventHandler<StrategyNotificationEventArgs> StrategyStatisticsEvent { add { } remove { } }
        public event EventHandler<StrategyNotificationEventArgs> StrategyParameterUpdateEvent { add { } remove { } }

        public bool AggregateTradesException { get; set; }
        public bool OrderBookException { get; set; }
        public Strategy Strategy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void SetStrategy(Strategy strategy)
        {
            throw new NotImplementedException();
        }

        public Task<Strategy> RunAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAccountInfoException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SubscribeTrades(TradeEventArgs aggregateTradeEventArgs)
        {
            throw new Exception("SubscribeAggregateTrades");
        }

        public void SubscribeTradesException(Exception exception)
        {
            AggregateTradesException = true;            
        }

        public void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs)
        {
            throw new Exception("SubscribeOrderBook");
        }

        public void SubscribeOrderBookException(Exception exception)
        {
            OrderBookException = true;
            throw new Exception("SubscribeOrderBookException");
        }

        public void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeStatisticsException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TryUpdateStrategyAsync(string strategyParameters)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TryStopStrategy(string strategyParameters)
        {
            throw new NotImplementedException();
        }

        public void SubscribeCandlesticks(AccountInfoEventArgs accountInfoEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeCandlesticksException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SubscribeCandlesticks(CandlestickEventArgs candlestickEventArgs)
        {
            throw new NotImplementedException();
        }

        public Task UpdateParametersAsync(string parameters)
        {
            throw new NotImplementedException();
        }

        public Task AddExchangeService(IEnumerable<StrategySubscription> strategySubscriptions, Exchange exchange, IExchangeService exchangeService)
        {
            throw new NotImplementedException();
        }
    }
}
