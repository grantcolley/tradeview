using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Events;
using DevelopmentInProgress.TradeView.Interface.Extensions;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Interface.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Interface.Strategy
{
    public abstract class TradeStrategyBase : ITradeStrategy
    {
        protected Dictionary<Exchange, IExchangeService> exchangeServices;
        protected Dictionary<Exchange, List<Symbol>> exchangeSymbols;
        protected bool run;

        protected AccountInfo accountInfo;
        protected object accountLock = new object();
        protected bool placingOrder;
        protected bool suspend;

        protected CancellationToken cancellationToken;

        protected TradeStrategyBase()
        {
            run = true;
            exchangeServices = new Dictionary<Exchange, IExchangeService>();
            exchangeSymbols = new Dictionary<Exchange, List<Symbol>>();
        }

        public event EventHandler<StrategyNotificationEventArgs> StrategyNotificationEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyAccountInfoEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyOrderBookEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyTradeEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyCandlesticksEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyStatisticsEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyCustomNotificationEvent;

        public Strategy Strategy { get; private set; }

        public virtual void SetStrategy(Strategy strategy)
        {
            var strategyXml = JsonConvert.SerializeObject(strategy);
            Strategy = JsonConvert.DeserializeObject<Strategy>(strategyXml);
        }

        public async virtual Task<Strategy> RunAsync(CancellationToken cancellationToken)
        {
            if(Strategy == null)
            {
                throw new Exception("Strategy not set.");
            }

            this.cancellationToken = cancellationToken;

            await TryUpdateStrategyAsync(Strategy.Parameters);

            while (run)
            {
                if (this.cancellationToken.IsCancellationRequested)
                {
                    run = false;
                }
                else
                {
                    await Task.Delay(500);
                }
            }

            var strategyNotification = new StrategyNotification { Name = Strategy.Name, Message = $"Stopping {Strategy.Name}", NotificationLevel = NotificationLevel.DisconnectClient };

            StrategyNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });

            return Strategy;
        }

        public async virtual Task AddExchangeService(IEnumerable<StrategySubscription> strategySubscriptions, Exchange exchange, IExchangeService exchangeService)
        {
            if (exchangeServices.ContainsKey(exchange))
            {
                return;
            }

            exchangeServices.Add(exchange, exchangeService);

            var symbols = await exchangeService.GetSymbolsAsync(exchange, cancellationToken);

            var subscribedSymbols = (from s in symbols
                                     join ss in strategySubscriptions on s.ExchangeSymbol equals ss.Symbol
                                     select s).ToList();

            exchangeSymbols.Add(exchange, subscribedSymbols);
        }

        public virtual Task<bool> TryStopStrategy(string strategyParameters)
        {
            var tcs = new TaskCompletionSource<bool>();
            
            suspend = true;
            run = false;

            tcs.SetResult(suspend);

            return tcs.Task;
        }

        public virtual async Task<bool> TryUpdateStrategyAsync(string strategyParameters)
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                var parameters = JsonConvert.DeserializeObject<StrategyParameters>(strategyParameters);

                suspend = parameters.Suspend;

                StrategyNotification(new StrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = Strategy.Name, Message = $"Parameter update : {parameters}", NotificationLevel = NotificationLevel.Information } });

                tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return await tcs.Task;
        }

        public virtual void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs)
        {
            lock (accountLock)
            {
                accountInfo = accountInfoEventArgs.AccountInfo.Clone();
                placingOrder = false;

                if (Strategy == null)
                {
                    return;
                }

                var strategyNotification = new StrategyNotification { Name = Strategy.Name, Message = "Update strategy performance.", NotificationLevel = NotificationLevel.Account };

                StrategyAccountInfoNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
            }
        }

        public virtual void SubscribeAccountInfoException(Exception exception)
        {
            var message = JsonConvert.SerializeObject(exception);

            var strategyNotification = new StrategyNotification { Name = Strategy.Name, Message = message, NotificationLevel = NotificationLevel.AccountError };

            StrategyAccountInfoNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        public virtual void SubscribeTrades(TradeEventArgs tradeEventArgs)
        {
            var message = JsonConvert.SerializeObject(tradeEventArgs.Trades);

            var strategyNotification = new StrategyNotification { Name = Strategy.Name, Message = message, NotificationLevel = NotificationLevel.Trade };

            StrategyTradeNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        public virtual void SubscribeTradesException(Exception exception)
        {
            var message = JsonConvert.SerializeObject(exception);

            var strategyNotification = new StrategyNotification { Name = Strategy.Name, Message = message, NotificationLevel = NotificationLevel.TradeError };

            StrategyTradeNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        public void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs)
        {
            var message = JsonConvert.SerializeObject(orderBookEventArgs.OrderBook);

            var strategyNotification = new StrategyNotification { Name = Strategy.Name, Message = message, NotificationLevel = NotificationLevel.OrderBook };

            StrategyOrderBookNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        public void SubscribeOrderBookException(Exception exception)
        {
            var message = JsonConvert.SerializeObject(exception);

            var strategyNotification = new StrategyNotification { Name = Strategy.Name, Message = message, NotificationLevel = NotificationLevel.OrderBookError };

            StrategyOrderBookNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        public virtual void SubscribeCandlesticks(CandlestickEventArgs candlestickEventArgs)
        {
            var message = JsonConvert.SerializeObject(candlestickEventArgs.Candlesticks);

            var strategyNotification = new StrategyNotification { Name = Strategy.Name, Message = message, NotificationLevel = NotificationLevel.Candlesticks };

            StrategyCandlesticksNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        public virtual void SubscribeCandlesticksException(Exception exception)
        {
            var message = JsonConvert.SerializeObject(exception);

            var strategyNotification = new StrategyNotification { Name = Strategy.Name, Message = message, NotificationLevel = NotificationLevel.CandlesticksError };

            StrategyCandlesticksNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        public virtual void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs)
        {
            var message = JsonConvert.SerializeObject(statisticsEventArgs.Statistics);

            var strategyNotification = new StrategyNotification { Name = Strategy.Name, Message = message, NotificationLevel = NotificationLevel.Statistics };

            StrategyStatisticsNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        public virtual void SubscribeStatisticsException(Exception exception)
        {
            var message = JsonConvert.SerializeObject(exception);

            var strategyNotification = new StrategyNotification { Name = Strategy.Name, Message = message, NotificationLevel = NotificationLevel.StatisticsError };

            StrategyStatisticsNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }

        protected void StrategyNotification(StrategyNotificationEventArgs e)
        {
            StrategyNotificationEvent?.Invoke(this, e);
        }

        protected void StrategyAccountInfoNotification(StrategyNotificationEventArgs e)
        {
            StrategyAccountInfoEvent?.Invoke(this, e);
        }

        protected void StrategyTradeNotification(StrategyNotificationEventArgs e)
        {
            StrategyTradeEvent?.Invoke(this, e);
        }

        protected void StrategyOrderBookNotification(StrategyNotificationEventArgs e)
        {
            StrategyOrderBookEvent?.Invoke(this, e);
        }

        protected void StrategyCandlesticksNotification(StrategyNotificationEventArgs e)
        {
            StrategyCandlesticksEvent?.Invoke(this, e);
        }

        protected void StrategyStatisticsNotification(StrategyNotificationEventArgs e)
        {
            StrategyStatisticsEvent?.Invoke(this, e);
        }

        protected void StrategyCustomNotification(StrategyNotificationEventArgs e)
        {
            StrategyCustomNotificationEvent?.Invoke(this, e);
        }
    }
}
