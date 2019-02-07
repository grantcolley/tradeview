using DevelopmentInProgress.Wpf.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.Common.Services;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        internal int ChartDisplayLimit => 500;
        internal int TradesDisplayLimit => 18;

        private CancellationTokenSource symbolCancellationTokenSource;
        private Symbol symbol;
        private OrderBook orderBook;
        private ChartValues<Trade> tradesChart;
        private ObservableCollection<Trade> trades;
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

        public ObservableCollection<Trade> Trades
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

                var tasks = new List<Task>(new [] { GetOrderBook(), GetTrades() }).ToArray();
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

        private void UpdateTrades(IEnumerable<Interface.Trade> trades)
        {
            lock (tradesLock)
            {
                if (Trades == null
                    || TradesChart == null)
                {
                    // Check - order by Id or Time
                    var orderedTrades = (from t in trades
                                         orderby t.Id
                                         select new Trade
                                         {
                                             Id = t.Id,
                                             Time = t.Time,
                                             Price = t.Price.Trim(Symbol.PricePrecision),
                                             Quantity = t.Quantity.Trim(Symbol.QuantityPrecision),
                                             IsBuyerMaker = t.IsBuyerMaker
                                         });

                    Action initialiseTrades = () =>
                    {
                        TradesChart = new ChartValues<Trade>(orderedTrades);

                        // CHECK - Take first or Skip to last display limit
                        Trades = new ObservableCollection<Trade>(orderedTrades.Take(TradesDisplayLimit));
                    };

                    if (Dispatcher == null)
                    {
                        initialiseTrades();
                    }
                    else
                    {
                        Dispatcher.Invoke(initialiseTrades);
                    }
                }
                else
                {
                    // TODO: save max Trade so no need to query for it again
                    var maxId = Trades.Max(at => at.Id);
                    var orderedTrades = (from t in trades
                                                  where t.Id > maxId
                                                  orderby t.Id
                                                  select new Trade
                                                  {
                                                      Id = t.Id,
                                                      Time = t.Time,
                                                      Price = t.Price.Trim(Symbol.PricePrecision),
                                                      Quantity = t.Quantity.Trim(Symbol.QuantityPrecision),
                                                      IsBuyerMaker = t.IsBuyerMaker
                                                  }).ToList();

                    var newCount = orderedTrades.Count;

                    if (TradesChart.Count >= ChartDisplayLimit)
                    {
                        var oldTrades = TradesChart.Take(newCount);
                        foreach(var oldTrade in oldTrades)
                        {
                            TradesChart.Remove(oldTrade);
                        }
                    }

                    Action updateTrades = () =>
                    {
                        TradesChart.AddRange(orderedTrades);

                        for (int i = 0; i < newCount; i++)
                        {
                            while (Trades.Count >= TradesDisplayLimit)
                            {
                                Trades.RemoveAt(Trades.Count - 1);
                            }

                            Trades.Insert(0, orderedTrades[i]);
                        }
                    };

                    if (Dispatcher == null)
                    {
                        updateTrades();
                    }
                    else
                    {
                        Dispatcher.Invoke(updateTrades);
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