using DevelopmentInProgress.Wpf.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.Common.Services;
using LiveCharts;
using LiveCharts.Configurations;
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

[assembly: InternalsVisibleTo("DevelopmentInProgress.Wpf.MarketView.Test")]
namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class SymbolViewModel : ExchangeViewModel
    {
        internal int Limit => 20;
        internal int ChartDisplayLimit => 100;
        internal int TradesDisplayLimit => 18;

        private CancellationTokenSource symbolCancellationTokenSource;
        private Symbol symbol;
        private OrderBook orderBook;
        private ChartValues<AggregateTrade> aggregateTradesChart;
        private ObservableCollection<AggregateTrade> aggregateTrades;
        private object orderBookLock = new object();
        private object aggregateTradesLock = new object();
        private bool isLoadingTrades;
        private bool isLoadingOrderBook;
        private bool disposed;

        public SymbolViewModel(IWpfExchangeService exchangeService)
            : base(exchangeService)
        {
            var mapper = Mappers.Xy<AggregateTrade>()
                .X(model => model.Time.Ticks)
                .Y(model => Convert.ToDouble(model.Price));

            Charting.For<AggregateTrade>(mapper);

            TimeFormatter = value => new DateTime((long)value).ToString("H:mm:ss");
            PriceFormatter = value => value.ToString("0.00000000");

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

        public ObservableCollection<AggregateTrade> AggregateTrades
        {
            get { return aggregateTrades; }
            set
            {
                if (aggregateTrades != value)
                {
                    aggregateTrades = value;
                    OnPropertyChanged("AggregateTrades");
                }
            }
        }

        public ChartValues<AggregateTrade> AggregateTradesChart
        {
            get { return aggregateTradesChart; }
            set
            {
                if (aggregateTradesChart != value)
                {
                    aggregateTradesChart = value;
                    OnPropertyChanged("AggregateTradesChart");
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
                AggregateTradesChart = null;
                AggregateTrades = null;
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
                var trades = await ExchangeService.GetAggregateTradesAsync(Symbol.Name, Limit, symbolCancellationTokenSource.Token);

                UpdateAggregateTrades(trades);

                ExchangeService.SubscribeAggregateTrades(Symbol.Name, Limit, e => UpdateAggregateTrades(e.AggregateTrades), SubscribeAggregateTradesException, symbolCancellationTokenSource.Token);
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

        private void UpdateAggregateTrades(IEnumerable<Interface.AggregateTrade> trades)
        {
            lock (aggregateTradesLock)
            {
                if (AggregateTrades == null
                    || AggregateTradesChart == null)
                {
                    var orderedTrades = (from t in trades
                                         orderby t.Id
                                         select new AggregateTrade
                                         {
                                             Id = t.Id,
                                             Time = t.Time,
                                             Price = t.Price.Trim(Symbol.PricePrecision),
                                             Quantity = t.Quantity.Trim(Symbol.QuantityPrecision),
                                             IsBuyerMaker = t.IsBuyerMaker
                                         });

                    Action initialiseAggregateTrades = () =>
                    {
                        AggregateTradesChart = new ChartValues<AggregateTrade>(orderedTrades); ;
                        AggregateTrades = new ObservableCollection<AggregateTrade>(orderedTrades.Take(TradesDisplayLimit));
                    };

                    if (Dispatcher == null)
                    {
                        initialiseAggregateTrades();
                    }
                    else
                    {
                        Dispatcher.Invoke(initialiseAggregateTrades);
                    }
                }
                else
                {
                    var maxId = AggregateTrades.Max(at => at.Id);
                    var orderedAggregateTrades = (from t in trades
                                                  where t.Id > maxId
                                                  orderby t.Id
                                                  select new AggregateTrade
                                                  {
                                                      Id = t.Id,
                                                      Time = t.Time,
                                                      Price = t.Price.Trim(Symbol.PricePrecision),
                                                      Quantity = t.Quantity.Trim(Symbol.QuantityPrecision),
                                                      IsBuyerMaker = t.IsBuyerMaker
                                                  }).ToList();

                    var newCount = orderedAggregateTrades.Count;

                    if (AggregateTradesChart.Count >= ChartDisplayLimit)
                    {
                        var oldTrades = AggregateTradesChart.Take(newCount);
                        foreach(var oldTrade in oldTrades)
                        {
                            AggregateTradesChart.Remove(oldTrade);
                        }
                    }

                    Action updateAggregateTrades = () =>
                    {
                        AggregateTradesChart.AddRange(orderedAggregateTrades);

                        for (int i = 0; i < newCount; i++)
                        {
                            while (AggregateTrades.Count >= TradesDisplayLimit)
                            {
                                AggregateTrades.RemoveAt(AggregateTrades.Count - 1);
                            }

                            AggregateTrades.Insert(0, orderedAggregateTrades[i]);
                        }
                    };

                    if (Dispatcher == null)
                    {
                        updateAggregateTrades();
                    }
                    else
                    {
                        Dispatcher.Invoke(updateAggregateTrades);
                    }
                }
            }
        }

        private void SubscribeAggregateTradesException(Exception exception)
        {
            OnException("SymbolViewModel.GetTrades - ExchangeService.SubscribeAggregateTrades", exception);
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