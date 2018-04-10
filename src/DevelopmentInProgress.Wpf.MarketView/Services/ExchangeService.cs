using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.Wpf.MarketView.Model;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.MarketView.Services
{
    public class ExchangeService : IExchangeService
    {
        private IExchangeApi exchangeApi;

        public ExchangeService(IExchangeApi exchangeApi)
        {
            this.exchangeApi = exchangeApi;
        }

        public async Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken)
        {
                var results = await exchangeApi.GetSymbolsAsync(cancellationToken);
                var symbols = results.Select(s => new Symbol
                {
                    NotionalMinimumValue = s.NotionalMinimumValue,
                    BaseAsset = s.BaseAsset,
                    Price = s.Price,
                    Quantity = s.Quantity,
                    QuoteAsset = s.QuoteAsset,
                    Status = s.Status,
                    IsIcebergAllowed = s.IsIcebergAllowed,
                    OrderTypes = s.OrderTypes,
                    SymbolStatistics = new SymbolStatistics { Symbol = $"{s.BaseAsset.Symbol}{s.QuoteAsset.Symbol}" }
                }).ToList();
                return symbols;
        }

        public async Task<IEnumerable<SymbolStatistics>> Get24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            var results = await exchangeApi.Get24HourStatisticsAsync(cancellationToken);
            var symbols = results.Select(s => new SymbolStatistics
            {
                FirstTradeId = s.FirstTradeId,
                CloseTime = s.CloseTime,
                OpenTime = s.OpenTime,
                QuoteVolume = s.QuoteVolume,
                Volume = Convert.ToInt64(s.Volume),
                LowPrice = s.LowPrice,
                HighPrice = s.HighPrice,
                OpenPrice = s.OpenPrice,
                AskQuantity = s.AskQuantity,
                AskPrice = s.AskPrice,
                BidQuantity = s.BidQuantity,
                BidPrice = s.BidPrice,
                LastQuantity = s.LastQuantity,
                LastPrice = s.LastPrice,
                PreviousClosePrice = s.PreviousClosePrice,
                WeightedAveragePrice = s.WeightedAveragePrice,
                PriceChangePercent = decimal.Round(s.PriceChangePercent, 2, MidpointRounding.AwayFromZero),
                PriceChange = s.PriceChange,
                Period = s.Period,
                Symbol = s.Symbol,
                LastTradeId = s.LastTradeId,
                TradeCount = s.TradeCount
            }).ToList();
            return symbols;
        }

        public async Task<IEnumerable<Symbol>> GetSymbols24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            var symbols = await GetSymbolsAsync(cancellationToken);
            var stats = await Get24HourStatisticsAsync(cancellationToken);

            Func<Symbol, SymbolStatistics, Symbol> f = ((sy, st) => 
            {
                sy.SymbolStatistics = st;
                sy.PriceChangePercentDirection = sy.SymbolStatistics.PriceChangePercent > 0 ? 1 : sy.SymbolStatistics.PriceChangePercent < 0 ? -1 : 0;
                return sy;
            });

            var updatedSymbols = (from sy in symbols
                                 join st in stats on sy.Name equals st.Symbol
                                 select f(sy,st)).ToList();
            return updatedSymbols;
        }

        public void SubscribeStatistics(IEnumerable<Symbol> symbols, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeStatistics(e =>
            {
                var stats = e.Statistics.ToList();
                Func<Symbol, Interface.SymbolStats, Symbol> f = ((sy, st) =>
                { 
                    sy.SymbolStatistics.PriceChangePercent = decimal.Round(st.PriceChangePercent, 2, MidpointRounding.AwayFromZero);
                    sy.PriceChangePercentDirection = sy.SymbolStatistics.PriceChangePercent > 0 ? 1 : sy.SymbolStatistics.PriceChangePercent < 0 ? -1 : 0;
                    sy.LastPriceChangeDirection = sy.SymbolStatistics.LastPrice > st.LastPrice ? 1 : sy.SymbolStatistics.LastPrice < st.LastPrice ? -1 : 0;
                    sy.SymbolStatistics.LastPrice = st.LastPrice;
                    sy.SymbolStatistics.Volume = Convert.ToInt64(st.Volume);
                    return sy;
                });

                (from sy in symbols join st in stats on sy.Name equals st.Symbol select f(sy, st)).ToList();

            }, exception, cancellationToken);
        }

        public async Task<IEnumerable<Interface.AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var aggregateTrades = await exchangeApi.GetAggregateTradesAsync(symbol, limit, cancellationToken);
            return aggregateTrades;
        }

        public async Task<Interface.OrderBook> GetOrderBookAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            var orderBook = await exchangeApi.GetOrderBookAsync(symbol, limit, cancellationToken);
            return orderBook;
        }

        public void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeOrderBook(symbol, limit, callback, exception, cancellationToken);
        }

        public async Task<Account> GetAccountInfoAsync(string apiKey, string apiSecret, CancellationToken cancellationToken)
        {
            var accountInfo = await exchangeApi.GetAccountInfoAsync(new Interface.User { ApiKey = apiKey, ApiSecret = apiSecret}, cancellationToken);
            return new Account(accountInfo);
        }

        public void SubscribeAggregateTrades(string symbol, int limit, Action<AggregateTradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeAggregateTrades(symbol, limit, callback, exception, cancellationToken);
        }

        public void SubscribeAccountInfo(Interface.User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            exchangeApi.SubscribeAccountInfo(user, callback, exception, cancellationToken);
        }
    }
}