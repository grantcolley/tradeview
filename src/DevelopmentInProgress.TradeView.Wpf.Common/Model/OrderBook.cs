using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class OrderBook : EntityBase
    {
        private string baseSymbol;
        private string quoteSymbol;
        public decimal bidAskSpread;
        private List<OrderBookPriceLevel> topAsks;
        private List<OrderBookPriceLevel> topBids;
        private ChartValues<OrderBookPriceLevel> chartAsks;
        private ChartValues<OrderBookPriceLevel> chartBids;
        private ChartValues<OrderBookPriceLevel> chartAggregatedAsks;
        private ChartValues<OrderBookPriceLevel> chartAggregatedBids;

        static OrderBook()
        {
            var mapper = Mappers.Xy<OrderBookPriceLevel>()
                 .X(model => Convert.ToDouble(model.Price))
                 .Y(model => Convert.ToDouble(model.Quantity));

            Charting.For<OrderBookPriceLevel>(mapper);
        }

        public OrderBook()
        {
            chartAsks = new ChartValues<OrderBookPriceLevel>();
            chartBids = new ChartValues<OrderBookPriceLevel>();
            chartAggregatedAsks = new ChartValues<OrderBookPriceLevel>();
            chartAggregatedBids = new ChartValues<OrderBookPriceLevel>();
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

        public List<Core.Model.OrderBookPriceLevel> Asks { get; set; }
        public List<Core.Model.OrderBookPriceLevel> Bids { get; set; }

        public List<OrderBookPriceLevel> TopAsks
        {
            get { return topAsks; }
            set
            {
                if(topAsks != value)
                {
                    topAsks = value;
                    OnPropertyChanged(nameof(TopAsks));
                }
            }
        }

        public ChartValues<OrderBookPriceLevel> ChartAsks
        {
            get { return chartAsks; }
            set
            {
                if (chartAsks != value)
                {
                    chartAsks = value;
                    OnPropertyChanged(nameof(ChartAsks));
                }
            }
        }

        public ChartValues<OrderBookPriceLevel> ChartAggregatedAsks
        {
            get { return chartAggregatedAsks; }
            set
            {
                if (chartAggregatedAsks != value)
                {
                    chartAggregatedAsks = value;
                    OnPropertyChanged(nameof(ChartAggregatedAsks));
                }
            }
        }

        public List<OrderBookPriceLevel> TopBids
        {
            get { return topBids; }
            set
            {
                if (topBids != value)
                {
                    topBids = value;
                    OnPropertyChanged(nameof(TopBids));
                }
            }
        }

        public ChartValues<OrderBookPriceLevel> ChartBids
        {
            get { return chartBids; }
            set
            {
                if (chartBids != value)
                {
                    chartBids = value;
                    OnPropertyChanged(nameof(ChartBids));
                }
            }
        }

        public ChartValues<OrderBookPriceLevel> ChartAggregatedBids
        {
            get { return chartAggregatedBids; }
            set
            {
                if (chartAggregatedBids != value)
                {
                    chartAggregatedBids = value;
                    OnPropertyChanged(nameof(ChartAggregatedBids));
                }
            }
        }

        public void Clear()
        {
            ChartAsks.Clear();
            ChartBids.Clear();
            ChartAggregatedAsks.Clear();
            ChartAggregatedBids.Clear();
            Asks = null;
            Bids = null;
            TopAsks = null;
            TopBids = null;
        }
    }
}