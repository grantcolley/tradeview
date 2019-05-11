using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    public interface ITradeStrategy
    {
        Task<Strategy> RunAsync(Strategy strategy, CancellationToken cancellationToken);

        event EventHandler<TradeStrategyNotificationEventArgs> StrategyAccountInfoEvent;
        
        event EventHandler<TradeStrategyNotificationEventArgs> StrategyNotificationEvent;

        event EventHandler<TradeStrategyNotificationEventArgs> StrategyOrderBookEvent;

        event EventHandler<TradeStrategyNotificationEventArgs> StrategyTradeEvent;

        void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs);

        void SubscribeStatisticsException(Exception exception);

        void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs);

        void SubscribeOrderBookException(Exception exception);

        void SubscribeTrades(TradeEventArgs tradeEventArgs);

        void SubscribeTradesException(Exception exception);

        void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs);

        void SubscribeAccountInfoException(Exception exception);

        void AddExchangeService(Exchange exchange, IExchangeService exchangeService);
    }
}