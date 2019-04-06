using DevelopmentInProgress.Wpf.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.Common.Services;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using System.Runtime.CompilerServices;
using DevelopmentInProgress.Wpf.Common.ViewModel;
using DevelopmentInProgress.Wpf.Common.Chart;
using System.Diagnostics;
using Prism.Logging;
using DevelopmentInProgress.MarketView.Interface.Interfaces;

[assembly: InternalsVisibleTo("DevelopmentInProgress.Wpf.MarketView.Test")]
namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class SymbolViewModel : ExchangeViewModel
    {
        private CancellationTokenSource symbolCancellationTokenSource;
        private Symbol symbol;
        private OrderBook orderBook;
        private ChartValues<TradeBase> tradesChart;
        private List<TradeBase> trades;
        private object orderBookLock = new object();
        private object tradesLock = new object();
        private bool isLoadingTrades;
        private bool isLoadingOrderBook;
        private bool disposed;

        public SymbolViewModel(IWpfExchangeService exchangeService, IChartHelper chartHelper, Preferences preferences, ILoggerFacade logger)
            : base(exchangeService, logger)
        {
            TradeLimit = preferences.TradeLimit;
            TradesDisplayCount = preferences.TradesDisplayCount;
            TradesChartDisplayCount = preferences.TradesChartDisplayCount;

            OrderBookLimit = preferences.OrderBookLimit;
            OrderBookDisplayCount = preferences.OrderBookDisplayCount;
            OrderBookChartDisplayCount = preferences.OrderBookChartDisplayCount;
            OrderBookCount = OrderBookChartDisplayCount > OrderBookDisplayCount ? OrderBookChartDisplayCount : OrderBookDisplayCount;

            TimeFormatter = chartHelper.TimeFormatter;
            PriceFormatter = chartHelper.PriceFormatter;

            OnPropertyChanged("");
        }

        public event EventHandler<SymbolEventArgs> OnSymbolNotification;

        internal int TradeLimit { get; }
        internal int TradesChartDisplayCount { get; }
        internal int TradesDisplayCount { get; }
        internal int OrderBookLimit { get; }
        internal int OrderBookChartDisplayCount { get; }
        internal int OrderBookDisplayCount { get; }
        internal int OrderBookCount { get; set; }

        public bool HasSymbol => Symbol != null ? true : false;

        public Func<double, string> TimeFormatter { get; set; }

        public Func<double, string> PriceFormatter { get; set; }

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

        public Symbol Symbol
        {
            get { return symbol; }
            set
            {
                if (symbol != value)
                {
                    symbol = value;
                    OnPropertyChanged("Symbol");
                    OnPropertyChanged("HasSymbol");
                }
            }
        }

        public List<TradeBase> Trades
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

        public ChartValues<TradeBase> TradesChart
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
                if (symbolCancellationTokenSource != null
                    && !symbolCancellationTokenSource.IsCancellationRequested)
                {
                    symbolCancellationTokenSource.Cancel();
                }
            }

            disposed = true;
        }

        public async Task SetSymbol(Symbol symbol)
        {
            try
            {
                if(symbolCancellationTokenSource != null
                    && !symbolCancellationTokenSource.IsCancellationRequested)
                {
                    symbolCancellationTokenSource.Cancel();
                }

                symbolCancellationTokenSource = new CancellationTokenSource();

                Symbol = symbol;
                TradesChart = null;
                Trades = null;
                OrderBook = null;

                var tasks = new List<Task>(new[] { GetOrderBook(), GetTrades()}).ToArray();

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                OnException("SymbolViewModel.SetSymbol", ex);
            }
        }

        private Stopwatch swOrderUpdate = new Stopwatch();
        private async Task GetOrderBook()
        {
            IsLoadingOrderBook = true;

            try
            {
                var orderBook = await ExchangeService.GetOrderBookAsync(Symbol.Name, OrderBookLimit, symbolCancellationTokenSource.Token);

                UpdateOrderBook(orderBook);

                swOrderUpdate.Start();
                ExchangeService.SubscribeOrderBook(Symbol.Name, OrderBookLimit, e => UpdateOrderBook(e.OrderBook), SubscribeOrderBookException, symbolCancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                OnException("SymbolViewModel.GetOrderBook", ex);
            }

            IsLoadingOrderBook = false;
        }

        private Stopwatch swTradeUpdate = new Stopwatch();
        private async Task GetTrades()
        {
            IsLoadingTrades = true;

            try
            {
                var trades = await ExchangeService.GetTradesAsync(Symbol.Name, TradeLimit, symbolCancellationTokenSource.Token);

                UpdateTrades(trades);

                swTradeUpdate.Start();
                ExchangeService.SubscribeTrades(Symbol.Name, TradeLimit, e => UpdateTrades(e.Trades), SubscribeTradesException, symbolCancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                OnException("SymbolViewModel.GetTrades", ex);
            }

            IsLoadingTrades = false;
        }

        internal void UpdateOrderBook(Interface.OrderBook orderBook)
        {
            if (!Symbol.Name.Equals(orderBook.Symbol))
            {
                throw new Exception("Orderbook update for wrong symbol");
            }

            lock (orderBookLock)
            {
                Logger.Log($"Orders New Update {swOrderUpdate.Elapsed}", Category.Info, Priority.Low);

                var sw = new Stopwatch();
                sw.Start();
                Logger.Log($"Start OrderBookUpdate", Category.Info, Priority.Low);

                bool firstOrders = false;

                if (OrderBook == null)
                {
                    // First incoming order book create the local order book.
                    firstOrders = true;

                    OrderBook = new OrderBook
                    {
                        Symbol = orderBook.Symbol,
                        BaseSymbol = Symbol.BaseAsset.Symbol,
                        QuoteSymbol = Symbol.QuoteAsset.Symbol
                    };
                }
                else if (OrderBook.LastUpdateId >= orderBook.LastUpdateId)
                {
                    // If the incoming order book is older than the local one ignore it.
                    return;
                }

                OrderBook.LastUpdateId = orderBook.LastUpdateId;
                OrderBook.Top = new OrderBookTop
                {
                    Ask = new OrderBookPriceLevel
                    {
                        Price = orderBook.Top.Ask.Price.Trim(Symbol.PricePrecision),
                        Quantity = orderBook.Top.Ask.Quantity.Trim(Symbol.QuantityPrecision)
                    },
                    Bid = new OrderBookPriceLevel
                    {
                        Price = orderBook.Top.Bid.Price.Trim(Symbol.PricePrecision),
                        Quantity = orderBook.Top.Bid.Quantity.Trim(Symbol.QuantityPrecision)
                    }
                };

                // Order by price: bids (DESC) and asks (ASC)
                var orderedAsks = orderBook.Asks.OrderBy(a => a.Price).ToList();
                var orderedBids = orderBook.Bids.OrderByDescending(b => b.Price).ToList();

                // The OrderBookCount is the greater of the OrderBookDisplayCount OrderBookChartDisplayCount.
                // Take the asks and bids for the OrderBookCount as new instances of type OrderBookPriceLevel 
                // i.e. discard those that we are not interested in displaying on the screen.
                var asks = orderedAsks.Take(OrderBookCount).Select(ask => new OrderBookPriceLevel
                {
                    Price = ask.Price.Trim(Symbol.PricePrecision),
                    Quantity = ask.Quantity.Trim(Symbol.QuantityPrecision)
                }).ToList();

                var bids = orderedBids.Take(OrderBookCount).Select(bid => new OrderBookPriceLevel
                {
                    Price = bid.Price.Trim(Symbol.PricePrecision),
                    Quantity = bid.Quantity.Trim(Symbol.QuantityPrecision)
                }).ToList();

                // Take the top bids and asks for the order book bid and ask lists and order descending.
                var topAsks = asks.Take(OrderBookDisplayCount).Reverse().ToList();
                var topBids = bids.Take(OrderBookDisplayCount).ToList();

                // Take the bid and aks to display in the the order book chart.
                var chartAsks = asks.Take(OrderBookChartDisplayCount).ToList();
                var chartBids = bids.Take(OrderBookChartDisplayCount).ToList();

                // Create the aggregated bids and asks for the aggregated bid and ask chart.
                var aggregatedAsks = chartAsks.GetAggregatedList();
                var aggregatedBids = chartBids.GetAggregatedList();

                // Create new instances of the top bids and asks, reversing the asks
                OrderBook.TopAsks = topAsks;
                OrderBook.TopBids = topBids;

                if (firstOrders)
                {
                    // Create new instances of the chart bids and asks, reversing the bids.
                    OrderBook.ChartAsks = new ChartValues<OrderBookPriceLevel>(chartAsks);
                    OrderBook.ChartBids = new ChartValues<OrderBookPriceLevel>(chartBids.Reverse<OrderBookPriceLevel>().ToList());
                    OrderBook.ChartAggregatedAsks = new ChartValues<OrderBookPriceLevel>(aggregatedAsks);
                    OrderBook.ChartAggregatedBids = new ChartValues<OrderBookPriceLevel>(aggregatedBids.Reverse<OrderBookPriceLevel>().ToList());
                }
                else
                {
                    // Update the existing orderbook chart bids and asks, reversing the bids.
                    OrderBook.UpdateChartAsks(chartAsks);
                    OrderBook.UpdateChartBids(chartBids.Reverse<OrderBookPriceLevel>().ToList());
                    OrderBook.UpdateChartAggregateAsks(aggregatedAsks);
                    OrderBook.UpdateChartAggregateBids(aggregatedBids.Reverse<OrderBookPriceLevel>().ToList());
                }

                sw.Stop();
                Logger.Log($"End OrderBookUpdate {sw.Elapsed}", Category.Info, Priority.Low);
                swOrderUpdate.Restart();
            }
        }

        internal void UpdateTrades(IEnumerable<ITrade> tradesUpdate)
        {
            lock (tradesLock)
            {
                Logger.Log($"Trade New Update {swTradeUpdate.Elapsed}", Category.Info, Priority.Low);
                
                var sw = new Stopwatch();
                sw.Start();
                Logger.Log($"Start TradeUpdate", Category.Info, Priority.Low);

                if (Trades == null)
                {
                    // First set of incoming trades
                    
                    // Order by oldest to newest (as it will appear in chart).
                    var newTrades = (from t in tradesUpdate
                                        orderby t.Time, t.Id
                                        select new TradeBase
                                        {
                                            Id = t.Id,
                                            Time = t.Time,
                                            Price = t.Price.Trim(Symbol.PricePrecision),
                                            Quantity = t.Quantity.Trim(Symbol.QuantityPrecision),
                                            IsBuyerMaker = t.IsBuyerMaker
                                        }).ToList();

                    var newTradesCount = newTrades.Count;

                    if (newTradesCount > TradesChartDisplayCount)
                    {
                        // More new trades than the chart can take, only takes the newest trades.
                        var chartTrades = newTrades.Skip(newTradesCount - TradesChartDisplayCount).ToList();
                        TradesChart = new ChartValues<TradeBase>(chartTrades);
                    }
                    else
                    {
                        // New trades less (or equal) the 
                        // total trades to show in the chart.
                        TradesChart = new ChartValues<TradeBase>(newTrades);
                    }

                    if (newTradesCount > TradesDisplayCount)
                    {
                        // More new trades than the list can take, only takes the newest trades.
                        var tradeBooktrades = newTrades.Skip(newTradesCount - TradesDisplayCount).ToList();

                        // Order by newest to oldest (as it will appear on trade list)
                        Trades = new List<TradeBase>(tradeBooktrades.Reverse<TradeBase>().ToList());
                    }
                    else
                    {
                        // New trades less (or equal) the 
                        // total trades to show in the trade list.
                        // Order by newest to oldest (as it will appear on trade list)
                        Trades = new List<TradeBase>(newTrades.Reverse<TradeBase>().ToList());
                    }
                }
                else
                {
                    // Subsequent set of new trades

                    // Get the latest available trade - the first trade on the 
                    // trade list (which is also the last trade in the chart).
                    var first = Trades.First();

                    // Extract new trades where time and id is greater than latest available trade. 
                    // Order by oldest to newest (as it will appear in chart).
                    var newTrades = (from t in tradesUpdate
                                     where t.Time > first.Time && t.Id > first.Id
                                     orderby t.Time, t.Id
                                     select new TradeBase
                                     {
                                         Id = t.Id,
                                         Time = t.Time,
                                         Price = t.Price.Trim(Symbol.PricePrecision),
                                         Quantity = t.Quantity.Trim(Symbol.QuantityPrecision),
                                         IsBuyerMaker = t.IsBuyerMaker
                                     }).ToList();

                    var newTradesCount = newTrades.Count;
                    var tradesChartCount = TradesChart.Count;

                    if (tradesChartCount >= TradesChartDisplayCount)
                    {
                        // For each additional new trade remove the oldest then add the new trade
                        for (int i = 0; i < newTradesCount; i++)
                        {
                            TradesChart.RemoveAt(0);
                            TradesChart.Add(newTrades[i]);
                        }
                    }
                    else
                    {
                        // Get the difference between the number of trades the chart can take and the number it currently holds.
                        var chartDisplayTopUpTradesCount = TradesChartDisplayCount - tradesChartCount;

                        if (newTradesCount > chartDisplayTopUpTradesCount)
                        {
                            // There are more new trades than the chart can take.

                            if (chartDisplayTopUpTradesCount > 0)
                            {
                                // The top up trades can simply be added to the chart as it will take it to the total the chart can hold
                                var chartDisplayTopUpTrades = newTrades.Take(chartDisplayTopUpTradesCount).ToList();
                                TradesChart.AddRange(chartDisplayTopUpTrades);
                            }

                            for (int i = chartDisplayTopUpTradesCount; i < newTradesCount; i++)
                            {
                                // For each additional new trade remove the oldest then add the new trade
                                TradesChart.RemoveAt(0);
                                TradesChart.Add(newTrades[i]);
                            }
                        }
                        else
                        {
                            // Simply add new trades to current list as it wont be more than the total the chart can take.
                            TradesChart.AddRange(newTrades);
                        }
                    }

                    if (newTradesCount >= TradesDisplayCount)
                    {
                        // More new trades than the list can take, only take the newest
                        // trades and create a new instance of the trades list.
                        var tradeBooktrades = newTrades.Skip(newTradesCount - TradesDisplayCount).ToList();

                        // Order by newest to oldest (as it will appear on trade list)
                        Trades = new List<TradeBase>(tradeBooktrades.Reverse<TradeBase>().ToList());
                    }
                    else
                    {
                        var tradesCount = Trades.Count;

                        // Order the new trades by newest first and oldest last
                        var tradeBooktrades = newTrades.Reverse<TradeBase>().ToList();

                        if ((newTradesCount + tradesCount) > TradesDisplayCount)
                        {
                            // Append to the new trades the balance from the existing trades to make up the trade list limit
                            var balanceTrades = Trades.Take(TradesDisplayCount - newTradesCount).ToList();
                            tradeBooktrades.AddRange(balanceTrades);
                        }
                        else
                        {
                            // Simply append the existing trades to the new trades as it will fit in the trade list limit.
                            tradeBooktrades.AddRange(Trades);
                        }

                        Trades = tradeBooktrades;
                    }
                }

                sw.Stop();
                Logger.Log($"End TradeUpdate {sw.Elapsed}", Category.Info, Priority.Low);
                swTradeUpdate.Restart();
            }
        }

        private void SubscribeTradesException(Exception exception)
        {
            OnException("SymbolViewModel.GetTrades - ExchangeService.SubscribeTrades", exception);
        }

        private void SubscribeOrderBookException(Exception exception)
        {
            OnException("SymbolViewModel.GetOrderBook - ExchangeService.SubscribeOrderBook", exception);
        }

        private void OnException(string message, Exception exception)
        {
            var onSymbolNotification = OnSymbolNotification;
            onSymbolNotification?.Invoke(this, new SymbolEventArgs { Message = message, Exception = exception });
        }
    }
}