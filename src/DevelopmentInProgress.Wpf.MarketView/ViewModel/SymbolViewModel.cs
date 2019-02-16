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
using System.Windows.Threading;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using System.Runtime.CompilerServices;
using DevelopmentInProgress.Wpf.Common.ViewModel;
using DevelopmentInProgress.Wpf.Common.Chart;

[assembly: InternalsVisibleTo("DevelopmentInProgress.Wpf.MarketView.Test")]
namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class SymbolViewModel : ExchangeViewModel
    {
        internal int Limit => 500;
        internal int TradesChartDisplayCount => 500;
        internal int TradesDisplayCount => 18;

        private CancellationTokenSource symbolCancellationTokenSource;
        private Symbol symbol;
        private OrderBook orderBook;
        private ChartValues<Trade> tradesChart;
        private List<Trade> trades;
        private object orderBookLock = new object();
        private object tradesLock = new object();
        private bool isLoadingTrades;
        private bool isLoadingOrderBook;
        private bool disposed;

        public SymbolViewModel(IWpfExchangeService exchangeService, IChartHelper chartHelper)
            : base(exchangeService)
        {
            TimeFormatter = chartHelper.TimeFormatter;
            PriceFormatter = chartHelper.PriceFormatter;

            OnPropertyChanged("");
        }

        public event EventHandler<SymbolEventArgs> OnSymbolNotification;

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

                var tasks = new List<Task>(new [] { /*GetOrderBook(),*/ GetTrades() }).ToArray();

                // don't await all - run on seperate tasks
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                OnException("SymbolViewModel.SetSymbol", ex);
            }
        }

        private async Task GetOrderBook()
        {
            IsLoadingOrderBook = true;

            try
            {
                var orderBook = await ExchangeService.GetOrderBookAsync(Symbol.Name, Limit, symbolCancellationTokenSource.Token);

                UpdateOrderBook(orderBook);

                ExchangeService.SubscribeOrderBook(Symbol.Name, Limit, e => UpdateOrderBook(e.OrderBook), SubscribeOrderBookException, symbolCancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                OnException("SymbolViewModel.GetOrderBook", ex);
            }

            IsLoadingOrderBook = false;
        }

        private async Task GetTrades()
        {
            IsLoadingTrades = true;

            try
            {
                var trades = await ExchangeService.GetTradesAsync(Symbol.Name, Limit, symbolCancellationTokenSource.Token);

                UpdateTrades(trades);

                ExchangeService.SubscribeTrades(Symbol.Name, Limit, e => UpdateTrades(e.Trades), SubscribeTradesException, symbolCancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                OnException("SymbolViewModel.GetTrades", ex);
            }

            IsLoadingTrades = false;
        }

        private void UpdateOrderBook(Interface.OrderBook orderBook)
        {
            if (!Symbol.Name.Equals(orderBook.Symbol))
            {
                return;
            }

            lock (orderBookLock)
            {
                if (OrderBook == null
                    || OrderBook.Symbol != Symbol.Name)
                {
                    OrderBook = new OrderBook
                    {
                        Symbol = orderBook.Symbol,
                        BaseSymbol = Symbol.BaseAsset.Symbol,
                        QuoteSymbol = Symbol.QuoteAsset.Symbol
                    };
                }

                if (OrderBook.LastUpdateId < orderBook.LastUpdateId)
                {
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

                    var asks = new List<OrderBookPriceLevel>(
                        (from ask in orderBook.Asks
                         orderby ask.Price
                         select new OrderBookPriceLevel
                         {
                             Price = ask.Price.Trim(Symbol.PricePrecision),
                             Quantity = ask.Quantity.Trim(Symbol.QuantityPrecision)
                         }));

                    var bids = new List<OrderBookPriceLevel>(
                        (from bid in orderBook.Bids
                         orderby bid.Price
                         select new OrderBookPriceLevel
                         {
                             Price = bid.Price.Trim(Symbol.PricePrecision),
                             Quantity = bid.Quantity.Trim(Symbol.QuantityPrecision)
                         }));

                    if (Dispatcher == null)
                    {
                        OrderBook.Update(asks, bids);
                    }
                    else
                    {
                        Dispatcher.Invoke(() => { OrderBook.Update(asks, bids); });
                    }
                }
            }
        }

        private void UpdateTrades(IEnumerable<Interface.Trade> tradesUpdate)
        {
            lock (tradesLock)
            {
                if (Trades == null)
                {
                    // First set of incoming trades
                    
                    // Order by oldest to newest (as it will appear in chart).
                    var newTrades = (from t in tradesUpdate
                                        orderby t.Time, t.Id
                                        select new Trade
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
                        var chartTrades = newTrades.Skip(newTradesCount - TradesChartDisplayCount);
                        TradesChart = new ChartValues<Trade>(chartTrades);
                    }
                    else
                    {
                        // New trades less (or equal) the 
                        // total trades to show in the chart.
                        TradesChart = new ChartValues<Trade>(newTrades);
                    }

                    if (newTradesCount > TradesDisplayCount)
                    {
                        // More new trades than the list can take, only takes the newest trades.
                        var tradeBooktrades = newTrades.Skip(newTradesCount - TradesDisplayCount).ToList();

                        // Order by newest to oldest (as it will appear on trade list)
                        Trades = new List<Trade>(tradeBooktrades.OrderByDescending(t => t.Time));
                    }
                    else
                    {
                        // New trades less (or equal) the 
                        // total trades to show in the trade list.
                        // Order by newest to oldest (as it will appear on trade list)
                        Trades = new List<Trade>(newTrades.OrderByDescending(t => t.Time));
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
                                        select new Trade
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

                        if(newTradesCount > chartDisplayTopUpTradesCount)
                        {
                            // There are more new trades than the chart can take.

                            if (chartDisplayTopUpTradesCount > 0)
                            {
                                // The top up trades can simply be added to the chart as it will take it to the total the chart can hold
                                var chartDisplayTopUpTrades = newTrades.Take(chartDisplayTopUpTradesCount);
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

                    if (newTradesCount > TradesDisplayCount)
                    {
                        // More new trades than the list can take, only takes the newest trades.
                        var tradeBooktrades = newTrades.Skip(newTradesCount - TradesDisplayCount);

                        // Order by newest to oldest (as it will appear on trade list)
                        Trades = new List<Trade>(tradeBooktrades.OrderByDescending(t => t.Time));
                    }
                    else
                    {
                        var tradesCount = Trades.Count;

                        // Order the new trades by newest first and oldest last
                        var tradeBooktrades = newTrades.OrderByDescending(t => t.Time).ToList();

                        if((newTradesCount + tradesCount) > TradesDisplayCount)
                        {
                            // Append to the new trades the balance from the existing trades to make up the trade list limit
                            tradeBooktrades.AddRange(Trades.Take(TradesDisplayCount - newTradesCount));
                        }
                        else
                        {
                            // Simply append the existing trades to the new trades as it will fit in the trade list limit.
                            tradeBooktrades.AddRange(Trades);
                        }
                        
                        Trades = tradeBooktrades;
                     }
                }
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