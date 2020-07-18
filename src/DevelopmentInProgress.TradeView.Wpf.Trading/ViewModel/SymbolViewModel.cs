using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Wpf.Common.Chart;
using DevelopmentInProgress.TradeView.Wpf.Common.Helpers;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Trading.Events;
using LiveCharts;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DevelopmentInProgress.TradeView.Wpf.Trading.Test")]
namespace DevelopmentInProgress.TradeView.Wpf.Trading.ViewModel
{
    public class SymbolViewModel : ExchangeViewModel
    {
        private CancellationTokenSource symbolCancellationTokenSource;
        private Symbol symbol;
        private OrderBook orderBook;
        private ChartValues<Trade> tradesChart;
        private List<Trade> trades;
        private Exchange exchange;
        private IOrderBookHelper orderBookHelper;
        private ITradeHelper tradeHelper;
        private SemaphoreSlim orderBookSemaphoreSlim = new SemaphoreSlim(1, 1);
        private SemaphoreSlim tradesSemaphoreSlim = new SemaphoreSlim(1, 1);
        private bool isLoadingTrades;
        private bool isLoadingOrderBook;
        private bool disposed;

        public SymbolViewModel(Exchange exchange, IWpfExchangeService exchangeService, IChartHelper chartHelper,
            IOrderBookHelper orderBookHelper, ITradeHelper tradeHelper, 
            Preferences preferences, ILoggerFacade logger)
            : base(exchangeService, logger)
        {
            this.exchange = exchange;
            this.orderBookHelper = orderBookHelper;
            this.tradeHelper = tradeHelper;

            TradeLimit = preferences.TradeLimit;
            TradesDisplayCount = preferences.TradesDisplayCount;
            TradesChartDisplayCount = preferences.TradesChartDisplayCount;

            ShowAggregateTrades = preferences.ShowAggregateTrades;

            OrderBookLimit = preferences.OrderBookLimit;
            OrderBookDisplayCount = preferences.OrderBookDisplayCount;
            OrderBookChartDisplayCount = preferences.OrderBookChartDisplayCount;

            TimeFormatter = chartHelper.TimeFormatter;
            PriceFormatter = chartHelper.PriceFormatter;

            symbolCancellationTokenSource = new CancellationTokenSource();

            OnPropertyChanged(string.Empty);
        }

        public event EventHandler<SymbolEventArgs> OnSymbolNotification;

        internal int TradeLimit { get; }
        internal int TradesChartDisplayCount { get; }
        internal int TradesDisplayCount { get; }
        internal bool ShowAggregateTrades { get; }
        internal int OrderBookLimit { get; }
        internal int OrderBookChartDisplayCount { get; }
        internal int OrderBookDisplayCount { get; }

        public bool IsActive { get; set; }

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

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                Unsubscribe();
            }

            disposed = true;
        }

        public async Task SetSymbol(Symbol symbol)
        {
            try
            {
                if (Symbol == null)
                {
                    Symbol = symbol;
                }
                else
                {
                    throw new Exception($"Attempting to replace {Symbol.ExchangeSymbol} with {symbol.ExchangeSymbol}");
                }
               
                await Task.WhenAll(SubscribeOrderBook(), SubscribeTrades());

                IsActive = true;
            }
            catch (Exception ex)
            {
                OnException("SymbolViewModel.SetSymbol", ex);
            }
        }

        private void Unsubscribe()
        {
            if (!symbolCancellationTokenSource.IsCancellationRequested)
            {
                symbolCancellationTokenSource.Cancel();
            }

            if (OrderBook != null)
            {
                OrderBook.Clear();
            }

            OrderBook = null;

            if (TradesChart != null)
            {
                TradesChart.Clear();
            }

            TradesChart = null;
            Trades = null;

            IsActive = false;
        }

        private async Task SubscribeOrderBook()
        {
            IsLoadingOrderBook = true;

            try
            {
                await ExchangeService.SubscribeOrderBook(exchange, Symbol.ExchangeSymbol, OrderBookLimit, e => UpdateOrderBook(e.OrderBook), SubscribeOrderBookException, symbolCancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                OnException("SymbolViewModel.GetOrderBook", ex);
            }
        }

        private async Task SubscribeTrades()
        {
            IsLoadingTrades = true;

            try
            {
                if (ShowAggregateTrades)
                {
                    await ExchangeService.SubscribeAggregateTrades(exchange, Symbol.ExchangeSymbol, TradeLimit, e => UpdateTrades(e.Trades), SubscribeTradesException, symbolCancellationTokenSource.Token);
                }
                else
                {
                    await ExchangeService.SubscribeTrades(exchange, Symbol.ExchangeSymbol, TradeLimit, e => UpdateTrades(e.Trades), SubscribeTradesException, symbolCancellationTokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                OnException("SymbolViewModel.GetTrades", ex);
            }
        }

        internal async void UpdateOrderBook(Core.Model.OrderBook exchangeOrderBook)
        {
            if(symbolCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            await orderBookSemaphoreSlim.WaitAsync(symbolCancellationTokenSource.Token);

            try
            {
                if (OrderBook == null)
                {
                    OrderBook = await orderBookHelper.CreateLocalOrderBook(Symbol, exchangeOrderBook, OrderBookDisplayCount, OrderBookChartDisplayCount);

                    if (IsLoadingOrderBook)
                    {
                        IsLoadingOrderBook = false;
                    }
                }
                else if (OrderBook.LastUpdateId >= exchangeOrderBook.LastUpdateId)
                {
                    // If the incoming order book is older than the local one ignore it.
                    return;
                }
                else
                {
                    orderBookHelper.UpdateLocalOrderBook(OrderBook, exchangeOrderBook,
                        symbol.PricePrecision, symbol.QuantityPrecision,
                        OrderBookDisplayCount, OrderBookChartDisplayCount);
                }
            }
            finally
            {
                orderBookSemaphoreSlim.Release();
            }
        }

        internal async void UpdateTrades(IEnumerable<ITrade> tradesUpdate)
        {
            if (symbolCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            await tradesSemaphoreSlim.WaitAsync(symbolCancellationTokenSource.Token);

            try
            {
                if (Trades == null)
                {
                    var result = await tradeHelper.CreateLocalTradeList<Trade>(Symbol, tradesUpdate, TradesDisplayCount, TradesChartDisplayCount, TradeLimit);

                    Trades = result.Trades;
                    TradesChart = result.TradesChart;

                    if (IsLoadingTrades)
                    {
                        IsLoadingTrades = false;
                    }
                }
                else
                {
                    List<Trade> newTrades;

                    tradeHelper.UpdateTrades(Symbol, tradesUpdate, Trades, TradesDisplayCount, TradesChartDisplayCount, out newTrades, ref tradesChart);

                    Trades = newTrades;
                }
            }
            finally
            {
                tradesSemaphoreSlim.Release();
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