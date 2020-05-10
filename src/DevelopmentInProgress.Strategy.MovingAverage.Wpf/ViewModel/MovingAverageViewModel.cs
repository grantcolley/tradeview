using DevelopmentInProgress.TradeView.Interface.Extensions;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Wpf.Common.Chart;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
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
using DevelopmentInProgress.TradeView.Wpf.Common.Helpers;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.Strategy.Common.StrategyTrade;

namespace DevelopmentInProgress.Strategy.MovingAverage.Wpf.ViewModel
{
    public class MovingAverageViewModel : StrategyDisplayViewModelBase
    {
        private ChartValues<Trade> tradesChart;
        private ChartValues<Trade> smaTradesChart;
        private ChartValues<Trade> buyIndicatorChart;
        private ChartValues<Trade> sellIndicatorChart;
        private ChartValues<Candlestick> candlesticksChart;
        private ObservableCollection<string> candlestickLabels;
        private List<Trade> trades;
        private OrderBook orderBook;
        private SemaphoreSlim tradesSemaphoreSlim = new SemaphoreSlim(1, 1);
        private SemaphoreSlim candlestickSemaphoreSlim = new SemaphoreSlim(1, 1);
        private SemaphoreSlim orderBookSemaphoreSlim = new SemaphoreSlim(1, 1);
        private CancellationTokenSource cancellationTokenSource;
        private bool isLoadingTrades;
        private bool isLoadingOrderBook;
        private bool disposed;
        private bool showCandlesticks;
        private ITradeHelperFactory tradeHelperFactory;
        private IOrderBookHelperFactory orderBookHelperFactory;

        public MovingAverageViewModel(WpfStrategy strategy, IHelperFactoryContainer iHelperFactoryContainer, 
            Dispatcher UiDispatcher, ILoggerFacade logger)
            : base(strategy, iHelperFactoryContainer, UiDispatcher, logger)
        {
            var chartHelper = ServiceLocator.Current.GetInstance<IChartHelper>();
            TimeFormatter = chartHelper.TimeFormatter;
            PriceFormatter = chartHelper.PriceFormatter;

            IsActive = false;
            IsLoadingTrades = true;
            IsLoadingOrderBook = true;

            cancellationTokenSource = new CancellationTokenSource();

            tradeHelperFactory = HelperFactoryContainer.GetFactory<ITradeHelperFactory>();

            orderBookHelperFactory = HelperFactoryContainer.GetFactory<IOrderBookHelperFactory>();

            ShowCandlesticks = Strategy.StrategySubscriptions.Any(s => s.SubscribeCandlesticks);
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

        public bool ShowCandlesticks
        {
            get { return showCandlesticks; }
            set
            {
                if (showCandlesticks != value)
                {
                    showCandlesticks = value;
                    OnPropertyChanged("ShowCandlesticks");
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
                cancellationTokenSource.Cancel();
            }

            disposed = true;
        }

        public override async Task TradeNotificationsAsync(List<StrategyNotification> tradeNotifications)
        {
            await tradesSemaphoreSlim.WaitAsync(cancellationTokenSource.Token);

            try
            {
                if(cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                if (Symbols != null)
                {
                    IsLoadingTrades = false;

                    List<MovingAverageTrade> tradesUpdate = null;

                    foreach (var notification in tradeNotifications)
                    {
                        if (tradesUpdate == null)
                        {
                            tradesUpdate = JsonConvert.DeserializeObject<List<MovingAverageTrade>>(notification.Message);
                            continue;
                        }

                        var updateTrades = JsonConvert.DeserializeObject<List<MovingAverageTrade>>(notification.Message);
                        var newTrades = updateTrades.Except(tradesUpdate).ToList();
                        tradesUpdate.AddRange(newTrades.OrderBy(t => t.Time));
                    }

                    var trade = tradesUpdate.First();

                    var symbol = Symbols.First(s => s.ExchangeSymbol.Equals(trade.Symbol));

                    var pricePrecision = symbol.PricePrecision;
                    var quantityPrecision = symbol.QuantityPrecision;

                    var tradeHelper = tradeHelperFactory.GetTradeHelper(trade.Exchange);

                    Func<ITrade, int, int, Trade> createSmaTrade = (t, p, q) => new Trade { Price = ((MovingAverageTrade)t).MovingAveragePrice.Trim(p), Time = t.Time.ToLocalTime(), Exchange = t.Exchange };
                    Func<ITrade, int, int, Trade> createBuyIndicator = (t, p, q) => new Trade { Price = ((MovingAverageTrade)t).BuyPrice.Trim(p), Time = t.Time.ToLocalTime(), Exchange = t.Exchange };
                    Func<ITrade, int, int, Trade> createSellIndicator = (t, p, q) => new Trade { Price = ((MovingAverageTrade)t).SellPrice.Trim(p), Time = t.Time.ToLocalTime(), Exchange = t.Exchange };

                    var tradesDisplayCount = Strategy.TradesDisplayCount;
                    var tradesChartDisplayCount = Strategy.TradesChartDisplayCount;

                    if (TradesChart == null)
                    {
                        var result = await tradeHelper.CreateLocalTradeList<Trade>(symbol, tradesUpdate, tradesDisplayCount, tradesChartDisplayCount, 0);

                        Trades = result.Trades;
                        TradesChart = result.TradesChart;

                        SmaTradesChart = tradeHelper.CreateLocalChartTrades(tradesUpdate, createSmaTrade, tradesChartDisplayCount, pricePrecision, quantityPrecision);

                        BuyIndicatorChart = tradeHelper.CreateLocalChartTrades(tradesUpdate, createBuyIndicator, tradesChartDisplayCount, pricePrecision, quantityPrecision);

                        SellIndicatorChart = tradeHelper.CreateLocalChartTrades(tradesUpdate, createSellIndicator, tradesChartDisplayCount, pricePrecision, quantityPrecision);
                    }
                    else
                    {
                        List<Trade> newTrades;

                        // Get the latest available trade - the first trade on the 
                        // trade list (which is also the last trade in the chart).
                        var seed = Trades.First();
                        var seedTime = seed.Time;
                        var seedId = seed.Id;

                        tradeHelper.UpdateTrades(symbol, tradesUpdate, Trades, tradesDisplayCount, tradesChartDisplayCount, out newTrades, ref tradesChart);

                        Trades = newTrades;

                        tradeHelper.UpdateLocalChartTrades(tradesUpdate, createSmaTrade, seedTime, seedId, tradesChartDisplayCount, pricePrecision, quantityPrecision, ref smaTradesChart);

                        tradeHelper.UpdateLocalChartTrades(tradesUpdate, createBuyIndicator, seedTime, seedId, tradesChartDisplayCount, pricePrecision, quantityPrecision, ref buyIndicatorChart);

                        tradeHelper.UpdateLocalChartTrades(tradesUpdate, createSellIndicator, seedTime, seedId, tradesChartDisplayCount, pricePrecision, quantityPrecision, ref sellIndicatorChart);
                    }
                }
            }
            catch(Exception ex)
            {
                OnException($"{Strategy.Name} : TradeNotificationsAsync - {ex.Message}", ex);
            }
            finally
            {
                tradesSemaphoreSlim.Release();
            }
        }

        public override async Task CandlestickNotificationsAsync(List<StrategyNotification> candlestickNotifications)
        {
            await candlestickSemaphoreSlim.WaitAsync(cancellationTokenSource.Token);

            try
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                var candlestickNotification = candlestickNotifications.Last();

                var cs = JsonConvert.DeserializeObject<List<TradeView.Interface.Model.Candlestick>>(candlestickNotification.Message);

                Candlestick last = null;

                if (CandlesticksChart != null)
                {
                    last = CandlesticksChart.LastOrDefault();
                }

                if (last == null)
                {
                    var candlesticks = cs.OrderBy(c => c.OpenTime).Select(c => c.ToViewCandlestick()).ToList();

                    CandlesticksChart = new ChartValues<Candlestick>(candlesticks);

                    var labels = candlesticks.Select(c => c.CloseTime.ToString("H:mm:ss"));
                    CandlestickLabels = new ObservableCollection<string>(labels);
                }
                else
                {
                    var candlesticks = cs.Where(c => c.OpenTime.ToLocalTime() >= last.OpenTime)
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
            catch (Exception ex)
            {
                OnException($"{Strategy.Name} : CandlestickNotificationsAsync - {ex.Message}", ex);
            }
            finally
            {
                candlestickSemaphoreSlim.Release();
            }

            await Task.FromResult<object>(null);
        }

        public override async Task OrderNotificationsAsync(List<StrategyNotification> orderNotifications)
        {
            await orderBookSemaphoreSlim.WaitAsync(cancellationTokenSource.Token);

            try
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                if (Symbols != null)
                {
                    IsLoadingOrderBook = false;

                    var orderBookDisplayCount = Strategy.OrderBookDisplayCount;
                    var orderBookChartDisplayCount = Strategy.OrderBookChartDisplayCount;

                    if (OrderBook == null)
                    {
                        var orderBookNotification = orderNotifications.Last();
                        
                        var ob = JsonConvert.DeserializeObject<TradeView.Interface.Model.OrderBook>(orderBookNotification.Message);

                        var orderBookHelper = orderBookHelperFactory.GetOrderBookHelper(ob.Exchange);

                        var symbol = Symbols.First(s => s.ExchangeSymbol.Equals(ob.Symbol));
                        var pricePrecision = symbol.PricePrecision;
                        var quantityPrecision = symbol.QuantityPrecision;

                        OrderBook = await orderBookHelper.CreateLocalOrderBook(symbol, ob, orderBookDisplayCount, orderBookChartDisplayCount);
                    }
                    else
                    {
                        bool first = true;
                        IOrderBookHelper orderBookHelper = null;
                        Symbol symbol = null;
                        int quantityPrecision;
                        int pricePrecision;

                        var orderBookUpdates = new List<TradeView.Interface.Model.OrderBook>();

                        foreach (var notification in orderNotifications)
                        {
                            var ob = JsonConvert.DeserializeObject<TradeView.Interface.Model.OrderBook>(notification.Message);

                            if (first)
                            {
                                orderBookHelper = orderBookHelperFactory.GetOrderBookHelper(ob.Exchange);

                                symbol = Symbols.First(s => s.ExchangeSymbol.Equals(ob.Symbol));
                                quantityPrecision = symbol.QuantityPrecision;
                                pricePrecision = symbol.PricePrecision;
                            }

                            orderBookUpdates.Add(ob);
                        }

                        var newOrderBookUpdates 
                            = orderBookUpdates
                            .OrderBy(ob => ob.LastUpdateId)
                            .Where(ob => ob.LastUpdateId > OrderBook.LastUpdateId).ToList();

                        foreach(var ob in newOrderBookUpdates)
                        {
                            orderBookHelper.UpdateLocalOrderBook(OrderBook, ob,
                                symbol.PricePrecision, symbol.QuantityPrecision,
                                orderBookDisplayCount, orderBookChartDisplayCount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnException($"{Strategy.Name} : OrderNotificationsAsync - {ex.Message}", ex);
            }
            finally
            {
                orderBookSemaphoreSlim.Release();
            }

            await Task.FromResult<object>(null);
        }
    }
}
