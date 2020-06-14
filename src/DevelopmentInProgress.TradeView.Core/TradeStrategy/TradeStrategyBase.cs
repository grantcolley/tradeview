using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Extensions;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Core.TradeStrategy
{
    public abstract class TradeStrategyBase : ITradeStrategy
    {
        protected TradeStrategyBase()
        {
            Run = true;
            ExchangeServices = new Dictionary<Exchange, IExchangeService>();
            ExchangeSymbols = new Dictionary<Exchange, List<Symbol>>();
        }

        public event EventHandler<StrategyNotificationEventArgs> StrategyNotificationEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyAccountInfoEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyOrderBookEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyTradeEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyCandlesticksEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyStatisticsEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyParameterUpdateEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyCustomNotificationEvent;

        public Strategy Strategy { get; private set; }

        protected object AccountLock { get; } = new object();
        protected CancellationToken CancellationToken { get; set; }
        protected Dictionary<Exchange, IExchangeService> ExchangeServices { get; }
        protected Dictionary<Exchange, List<Symbol>> ExchangeSymbols { get; }
        protected AccountInfo AccountInfo { get; set; }
        protected bool Run { get; set; }
        protected bool PlacingOrder { get; set; }
        protected bool Suspend { get; set; }

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

            this.CancellationToken = cancellationToken;

            await TryUpdateStrategyAsync(Strategy.Parameters).ConfigureAwait(false);

            while (Run)
            {
                if (this.CancellationToken.IsCancellationRequested)
                {
                    Run = false;
                }
                else
                {
                    await Task.Delay(500).ConfigureAwait(false);
                }
            }

            var strategyNotification = new StrategyNotification { Name = Strategy.Name, Message = $"Stopping {Strategy.Name}", NotificationLevel = NotificationLevel.DisconnectClient };
            StrategyNotification(new StrategyNotificationEventArgs { StrategyNotification = strategyNotification });

            return Strategy;
        }

        public async virtual Task AddExchangeService(IEnumerable<StrategySubscription> strategySubscriptions, Exchange exchange, IExchangeService exchangeService)
        {
            if(exchangeService == null)
            {
                throw new ArgumentNullException(nameof(exchangeService));
            }

            if (ExchangeServices.ContainsKey(exchange))
            {
                return;
            }

            ExchangeServices.Add(exchange, exchangeService);

            var symbols = await exchangeService.GetSymbolsAsync(exchange, CancellationToken).ConfigureAwait(false);

            var subscribedSymbols = (from s in symbols
                                     join ss in strategySubscriptions on s.ExchangeSymbol equals ss.Symbol
                                     select s).ToList();

            ExchangeSymbols.Add(exchange, subscribedSymbols);
        }

        public virtual Task<bool> TryStopStrategy(string strategyParameters)
        {
            var tcs = new TaskCompletionSource<bool>();
            
            Suspend = true;
            Run = false;

            tcs.SetResult(Suspend);

            return tcs.Task;
        }

        public virtual async Task<bool> TryUpdateStrategyAsync(string strategyParameters)
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                var parameters = JsonConvert.DeserializeObject<StrategyParameters>(strategyParameters);

                Suspend = parameters.Suspend;

                StrategyParameterUpdateNotification(new StrategyNotificationEventArgs { StrategyNotification = new StrategyNotification { Name = Strategy.Name, Message = strategyParameters, NotificationLevel = NotificationLevel.Information } });

                tcs.SetResult(true);
            }
            catch (JsonException ex)
            {
                tcs.SetResult(false);
                tcs.SetException(ex);
            }

            return await tcs.Task.ConfigureAwait(false);
        }

        public virtual void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs)
        {
            if (accountInfoEventArgs == null)
            {
                throw new ArgumentNullException(nameof(accountInfoEventArgs));
            }

            lock (AccountLock)
            {
                AccountInfo = accountInfoEventArgs.AccountInfo.Clone();
                PlacingOrder = false;

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
            if (tradeEventArgs == null)
            {
                throw new ArgumentNullException(nameof(tradeEventArgs));
            }

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
            if (orderBookEventArgs == null)
            {
                throw new ArgumentNullException(nameof(orderBookEventArgs));
            }

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
            if (candlestickEventArgs == null)
            {
                throw new ArgumentNullException(nameof(candlestickEventArgs));
            }

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
            if (statisticsEventArgs == null)
            {
                throw new ArgumentNullException(nameof(statisticsEventArgs));
            }

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

        protected void StrategyParameterUpdateNotification(StrategyNotificationEventArgs e)
        {
            StrategyParameterUpdateEvent?.Invoke(this, e);
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
