using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    public interface ITradeStrategy
    {
        Task<Strategy> RunAsync(Strategy strategy, CancellationToken cancellationToken);

        Task<bool> TryUpdateStrategy(string strategyParameters);

        event EventHandler<StrategyNotificationEventArgs> StrategyNotificationEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyAccountInfoEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyOrderBookEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyTradeEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyCandlesticksEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyStatisticsEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyCustomNotificationEvent;

        void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs);

        void SubscribeStatisticsException(Exception exception);

        void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs);

        void SubscribeOrderBookException(Exception exception);

        void SubscribeTrades(TradeEventArgs tradeEventArgs);

        void SubscribeTradesException(Exception exception);

        void SubscribeCandlesticks(CandlestickEventArgs candlestickEventArgs);

        void SubscribeCandlesticksException(Exception exception);

        void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs);

        void SubscribeAccountInfoException(Exception exception);

        Task AddExchangeService(IEnumerable<StrategySubscription> strategySubscriptions, Exchange exchange, IExchangeService exchangeService);
    }
}