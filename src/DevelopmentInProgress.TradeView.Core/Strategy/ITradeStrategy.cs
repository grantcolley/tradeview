using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Core.Strategy
{
    public interface ITradeStrategy
    {
        Strategy Strategy { get; }

        event EventHandler<StrategyNotificationEventArgs> StrategyNotificationEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyAccountInfoEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyOrderBookEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyTradeEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyCandlesticksEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyStatisticsEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyCustomNotificationEvent;

        event EventHandler<StrategyNotificationEventArgs> StrategyParameterUpdateEvent;

        void SetStrategy(Strategy strategy);

        Task<Strategy> RunAsync(CancellationToken cancellationToken);

        Task AddExchangeService(IEnumerable<StrategySubscription> strategySubscriptions, Exchange exchange, IExchangeService exchangeService);

        Task<bool> TryStopStrategy(string strategyParameters);

        Task<bool> TryUpdateStrategyAsync(string strategyParameters);

        void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs);

        void SubscribeAccountInfoException(Exception exception);

        void SubscribeTrades(TradeEventArgs tradeEventArgs);

        void SubscribeTradesException(Exception exception);

        void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs);

        void SubscribeOrderBookException(Exception exception);

        void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs);

        void SubscribeStatisticsException(Exception exception);

        void SubscribeCandlesticks(CandlestickEventArgs candlestickEventArgs);

        void SubscribeCandlesticksException(Exception exception);
    }
}