using DevelopmentInProgress.TradeView.Interface.Extensions;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Wpf.Common.Chart;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Helpers;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using WpfStrategy = DevelopmentInProgress.TradeView.Wpf.Common.Model.Strategy;
using LiveCharts;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace DevelopmentInProgress.Strategy.Demo.Wpf.ViewModel
{
    public class DemoViewModel : StrategyDisplayViewModelBase
    {
        private ChartValues<Trade> tradesChart;
        private ChartValues<Trade> smaTradesChart;
        private ChartValues<Trade> buyIndicatorChart;
        private ChartValues<Trade> sellIndicatorChart;
        private ChartValues<Candlestick> candlesticksChart;
        private ObservableCollection<string> candlestickLabels;
        private List<Trade> trades;
        private OrderBook orderBook;
        private object tradesLock = new object();
        private object orderBookLock = new object();
        private object candlestickLock = new object();
        private bool isLoadingTrades;
        private bool isLoadingOrderBook;
        private bool disposed;

        public DemoViewModel(WpfStrategy strategy, Dispatcher UiDispatcher, ILoggerFacade logger)
            : base(strategy, UiDispatcher, logger)
        {
            var chartHelper = ServiceLocator.Current.GetInstance<IChartHelper>();
            TimeFormatter = chartHelper.TimeFormatter;
            PriceFormatter = chartHelper.PriceFormatter;

            IsActive = false;
            IsLoadingTrades = true;
            IsLoadingOrderBook = true;
        }

        public Func<double, string> TimeFormatter { get; set; }
        public Func<double, string> PriceFormatter { get; set; }

        public bool IsLoadingTrades
        {
            get { return isLoadingTrades; }
            set
            {
                if (isLoadingTrades != value)
                {
                    isLoadingTrades = value;
                    OnPropertyChanged("IsLoadingTrades");
                }
            }
        }

        public bool IsLoadingOrderBook
        {
            get { return isLoadingOrderBook; }
            set
            {
                if (isLoadingOrderBook != value)
                {
                    isLoadingOrderBook = value;
                    OnPropertyChanged("IsLoadingOrderBook");
                }
            }
        }

        public ChartValues<Trade> TradesChart
        {
            get { return tradesChart; }
            set
            {
                if (tradesChart != value)
                {
                    tradesChart = value;
                    OnPropertyChanged("TradesChart");
                }
            }
        }

        public ChartValues<Trade> SmaTradesChart
        {
            get { return smaTradesChart; }
            set
            {
                if (smaTradesChart != value)
                {
                    smaTradesChart = value;
                    OnPropertyChanged("SmaTradesChart");
                }
            }
        }

        public ChartValues<Trade> BuyIndicatorChart
        {
            get { return buyIndicatorChart; }
            set
            {
                if (buyIndicatorChart != value)
                {
                    buyIndicatorChart = value;
                    OnPropertyChanged("BuyIndicatorChart");
                }
            }
        }

        public ChartValues<Trade> SellIndicatorChart
        {
            get { return sellIndicatorChart; }
            set
            {
                if (sellIndicatorChart != value)
                {
                    sellIndicatorChart = value;
                    OnPropertyChanged("SellIndicatorChart");
                }
            }
        }

        public ChartValues<Candlestick> CandlesticksChart
        {
            get { return candlesticksChart; }
            set
            {
                if (candlesticksChart != value)
                {
                    candlesticksChart = value;
                    OnPropertyChanged("CandlesticksChart");
                }
            }
        }

        public ObservableCollection<string> CandlestickLabels
        {
            get { return candlestickLabels; }
            set
            {
                candlestickLabels = value;
                OnPropertyChanged("CandlestickLabels");
            }
        }

        public List<Trade> Trades
        {
            get { return trades; }
            set
            {
                if (trades != value)
                {
                    trades = value;
                    OnPropertyChanged("Trades");
                }
            }
        }

        public OrderBook OrderBook
        {
            get { return orderBook; }
            set
            {
                if (orderBook != value)
                {
                    orderBook = value;
                    OnPropertyChanged("OrderBook");
                }
            }
        }

        public override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // do stuff...
            }

            disposed = true;
        }

        public override void TradeNotifications(List<StrategyNotification> tradeNotifications)
        {
            List<DemoTrade> tradesUpdate = null;

            foreach (var notification in tradeNotifications)
            {
                if (tradesUpdate == null)
                {
                    tradesUpdate = JsonConvert.DeserializeObject<List<DemoTrade>>(notification.Message);
                    continue;
                }

                var updateTrades = JsonConvert.DeserializeObject<List<DemoTrade>>(notification.Message);
                var newTrades = updateTrades.Except(tradesUpdate).ToList();
                tradesUpdate.AddRange(newTrades);
            }

            if (Symbols != null)
            {
                lock (tradesLock)
                {
                    IsLoadingTrades = false;

                    var trade = tradesUpdate.First();

                    var symbol = Symbols.First(s => s.Name.Equals(trade.Symbol));

                    var pricePrecision = symbol.PricePrecision;

                    List<DemoTrade> smaTradesUpdate;

                    if(tradesUpdate.Count > Strategy.TradesChartDisplayCount)
                    {
                        smaTradesUpdate = tradesUpdate.Skip(tradesUpdate.Count - Strategy.TradesChartDisplayCount).ToList();
                    }
                    else
                    {
                        smaTradesUpdate = tradesUpdate;
                    }

                    Func<ITrade, Trade> createTrade = t => new Trade { Price = t.Price.Trim(symbol.PricePrecision), Quantity = t.Quantity.Trim(symbol.QuantityPrecision), Time = t.Time.ToLocalTime() };
                    Func<ITrade, Trade> createSmaTrade = t => new Trade { Price = ((DemoTrade)t).SmaPrice.Trim(symbol.PricePrecision), Time = t.Time.ToLocalTime() };
                    Func<ITrade, Trade> createBuyIndicator = t => new Trade { Price = ((DemoTrade)t).BuyIndicatorPrice.Trim(symbol.PricePrecision), Time = t.Time.ToLocalTime() };
                    Func<ITrade, Trade> createSellIndicator = t => new Trade { Price = ((DemoTrade)t).SellIndicatorPrice.Trim(symbol.PricePrecision), Time = t.Time.ToLocalTime() };

                    var tradesChartDisplayCount = Strategy.TradesChartDisplayCount;

                    if (TradesChart == null)
                    {
                        Trades = TradeHelper.GetNewTradesList(tradesUpdate, createTrade, Strategy.TradesDisplayCount);

                        TradesChart = TradeHelper.GetNewTradesChart(tradesUpdate, createTrade, tradesChartDisplayCount);

                        SmaTradesChart = TradeHelper.GetNewTradesChart(tradesUpdate, createSmaTrade, tradesChartDisplayCount);

                        BuyIndicatorChart = TradeHelper.GetNewTradesChart(tradesUpdate, createBuyIndicator, tradesChartDisplayCount);

                        SellIndicatorChart = TradeHelper.GetNewTradesChart(tradesUpdate, createSellIndicator, tradesChartDisplayCount);
                    }
                    else
                    {
                        Trades = TradeHelper.GetUpdatedTradesList(tradesUpdate, Trades, createTrade, Strategy.TradesDisplayCount);

                        TradeHelper.UpdateTradesChart(tradesUpdate, createTrade, tradesChartDisplayCount, ref tradesChart);

                        TradeHelper.UpdateTradesChart(tradesUpdate, createSmaTrade, tradesChartDisplayCount, ref smaTradesChart);

                        TradeHelper.UpdateTradesChart(tradesUpdate, createBuyIndicator, tradesChartDisplayCount, ref buyIndicatorChart);

                        TradeHelper.UpdateTradesChart(tradesUpdate, createSellIndicator, tradesChartDisplayCount, ref sellIndicatorChart);
                    }
                }
            }
        }

        public override void CandlestickNotifications(List<StrategyNotification> candlestickNotifications)
        {
            if (TradesChart == null
                || !TradesChart.Any())
            {
                return;
            }

            var candlestickNotification = candlestickNotifications.Last();

            lock(candlestickLock)
            {
                var csjson = JsonConvert.DeserializeObject<List<TradeView.Interface.Model.Candlestick>>(candlestickNotification.Message);

                Candlestick last = null;

                if (CandlesticksChart != null)
                {
                    last = CandlesticksChart.LastOrDefault();
                }

                if(last == null)
                {
                    var firstTrade = TradesChart.First();

                    var cs = csjson.Where(c => c.OpenTime.ToLocalTime() > firstTrade.Time).OrderBy(c => c.OpenTime).ToList();

                    var candlesticks = cs.Select(c => c.ToViewCandlestick()).ToList();

                    CandlesticksChart = new ChartValues<Candlestick>(candlesticks);

                    var labels = candlesticks.Select(c => c.CloseTime.ToString("H:mm:ss"));
                    CandlestickLabels = new ObservableCollection<string>(labels);
                }
                else
                {
                    var candlesticks = csjson.Where(c => c.OpenTime.ToLocalTime() >= last.OpenTime)
                        .OrderBy(c => c.OpenTime)
                        .ToList();

                    var first = candlesticks.FirstOrDefault();

                    if (first != null
                        && first.OpenTime.ToLocalTime().Equals(last.OpenTime))
                    {
                        last.High = first.High;
                        last.Low = first.Low;
                        last.Close = first.Close;
                        last.CloseTime = first.CloseTime.ToLocalTime();
                        last.Volume = first.Volume;
                        last.QuoteAssetVolume = first.QuoteAssetVolume;
                        last.TakerBuyBaseAssetVolume = first.TakerBuyBaseAssetVolume;
                        last.TakerBuyQuoteAssetVolume = first.TakerBuyQuoteAssetVolume;
                        last.NumberOfTrades = first.NumberOfTrades;
                    }

                    if (candlesticks.Count > 1)
                    {
                        var newCandlesticks = candlesticks.Skip(1).Select(c => c.ToViewCandlestick()).ToList();

                        CandlesticksChart.AddRange(newCandlesticks);

                        var labels = newCandlesticks.Select(c => c.CloseTime.ToString("H:mm:ss"));
                        CandlestickLabels.AddRange(labels);
                    }
                }
            }
        }

        public override void OrderNotifications(List<StrategyNotification> orderNotifications)
        {
            var orderBookNotification = orderNotifications.Last();

            lock (orderBookLock)
            {
                IsLoadingOrderBook = false;

                var ob = JsonConvert.DeserializeObject<TradeView.Interface.Model.OrderBook>(orderBookNotification.Message);

                bool firstOrders = false;

                var symbol = Symbols.Single(s => s.Name.Equals(ob.Symbol));

                if (OrderBook == null)
                {
                    // First incoming order book create the local order book.
                    firstOrders = true;

                    OrderBook = new OrderBook
                    {
                        Symbol = ob.Symbol,
                        BaseSymbol = symbol.BaseAsset.Symbol,
                        QuoteSymbol = symbol.QuoteAsset.Symbol
                    };
                }
                else if (OrderBook.LastUpdateId >= ob.LastUpdateId)
                {
                    // If the incoming order book is older than the local one ignore it.
                    return;
                }

                OrderBook.LastUpdateId = ob.LastUpdateId;

                // Order by price: bids (DESC) and asks (ASC)
                var orderedAsks = ob.Asks.OrderBy(a => a.Price).ToList();
                var orderedBids = ob.Bids.OrderByDescending(b => b.Price).ToList();

                // Take the asks and bids for the OrderBookChartDisplayCount as new instances of type OrderBookPriceLevel 
                // i.e. discard those that we are not interested in displaying on the screen.
                var asks = orderedAsks.Take(Strategy.OrderBookChartDisplayCount).Select(ask => new OrderBookPriceLevel
                {
                    Price = ask.Price.Trim(symbol.PricePrecision),
                    Quantity = ask.Quantity.Trim(symbol.QuantityPrecision)
                }).ToList();

                var bids = orderedBids.Take(Strategy.OrderBookChartDisplayCount).Select(bid => new OrderBookPriceLevel
                {
                    Price = bid.Price.Trim(symbol.PricePrecision),
                    Quantity = bid.Quantity.Trim(symbol.QuantityPrecision)
                }).ToList();

                // Take the bid and aks to display in the the order book chart.
                var chartAsks = asks.Take(Strategy.OrderBookChartDisplayCount).ToList();
                var chartBids = bids.Take(Strategy.OrderBookChartDisplayCount).ToList();

                // Create the aggregated bids and asks for the aggregated bid and ask chart.
                //var aggregatedAsks = chartAsks.GetAggregatedList();
                //var aggregatedBids = chartBids.GetAggregatedList();

                //if (firstOrders)
                //{
                //    // Create new instances of the chart bids and asks, reversing the bids.
                //    OrderBook.ChartAggregatedAsks = new ChartValues<OrderBookPriceLevel>(aggregatedAsks);
                //    OrderBook.ChartAggregatedBids = new ChartValues<OrderBookPriceLevel>(aggregatedBids.Reverse<OrderBookPriceLevel>().ToList());
                //}
                //else
                //{
                //    // Update the existing orderbook chart bids and asks, reversing the bids.
                //    OrderBook.UpdateChartAggregateAsks(aggregatedAsks);
                //    OrderBook.UpdateChartAggregateBids(aggregatedBids.Reverse<OrderBookPriceLevel>().ToList());
                //}
            }
        }
    }
}
