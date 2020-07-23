using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
    public class OrderBook : EntityBase
    {
        private string baseSymbol;
        private string quoteSymbol;
        private decimal bidAskSpread;

        static OrderBook()
        {
            var mapper = Mappers.Xy<OrderBookPriceLevel>()
                 .X(model => Convert.ToDouble(model.Price))
                 .Y(model => Convert.ToDouble(model.Quantity));

            Charting.For<OrderBookPriceLevel>(mapper);
        }

        public OrderBook()
        {
            Asks = new List<Core.Model.OrderBookPriceLevel>();
            Bids = new List<Core.Model.OrderBookPriceLevel>();
            TopAsks = new List<OrderBookPriceLevel>();
            TopBids = new List<OrderBookPriceLevel>();
            ChartAsks = new ChartValues<OrderBookPriceLevel>();
            ChartBids = new ChartValues<OrderBookPriceLevel>();
            ChartAggregatedAsks = new ChartValues<OrderBookPriceLevel>();
            ChartAggregatedBids = new ChartValues<OrderBookPriceLevel>();
        }

        public string Symbol { get; set; }
        public long LastUpdateId { get; set; }

        public string BaseSymbol
        {
            get { return baseSymbol; }
            set
            {
                if (baseSymbol != value)
                {
                    baseSymbol = value;
                    OnPropertyChanged(nameof(BaseSymbol));
                }
            }
        }

        public string QuoteSymbol
        {
            get { return quoteSymbol; }
            set
            {
                if (quoteSymbol != value)
                {
                    quoteSymbol = value;
                    OnPropertyChanged(nameof(QuoteSymbol));
                }
            }
        }

        public decimal BidAskSpread
        {
            get { return bidAskSpread; }
            set
            {
                if (bidAskSpread != value)
                {
                    bidAskSpread = value;
                    OnPropertyChanged(nameof(BidAskSpread));
                }
            }
        }

        public List<Core.Model.OrderBookPriceLevel> Asks { get; }
        public List<Core.Model.OrderBookPriceLevel> Bids { get; }
        public List<OrderBookPriceLevel> TopAsks { get; }
        public ChartValues<OrderBookPriceLevel> ChartAsks { get; }
        public ChartValues<OrderBookPriceLevel> ChartAggregatedAsks { get; }
        public List<OrderBookPriceLevel> TopBids { get; }
        public ChartValues<OrderBookPriceLevel> ChartBids { get; }
        public ChartValues<OrderBookPriceLevel> ChartAggregatedBids { get; }

        public void Clear()
        {
            ChartAsks.Clear();
            ChartBids.Clear();
            ChartAggregatedAsks.Clear();
            ChartAggregatedBids.Clear();
            Asks.Clear();
            Bids.Clear();
            TopAsks.Clear();
            TopBids.Clear();
        }
    }
}