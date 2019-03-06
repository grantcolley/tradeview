using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentInProgress.Wpf.Common.Model
{
    public class OrderBook : EntityBase
    {
        private string baseSymbol;
        private string quoteSymbol;
        private OrderBookTop top;
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
                    OnPropertyChanged("BaseSymbol");
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
                    OnPropertyChanged("QuoteSymbol");
                }
            }
        }

        public OrderBookTop Top
        {
            get { return top; }
            set
            {
                if (top != value)
                {
                    top = value;
                    OnPropertyChanged("Top");
                }
            }
        }

        public List<OrderBookPriceLevel> TopAsks
        {
            get { return topAsks; }
            set
            {
                if(topAsks != value)
                {
                    topAsks = value;
                    OnPropertyChanged("TopAsks");
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
                    OnPropertyChanged("ChartAsks");
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
                    OnPropertyChanged("ChartAggregatedAsks");
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
                    OnPropertyChanged("TopBids");
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
                    OnPropertyChanged("ChartBids");
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
                    OnPropertyChanged("ChartAggregatedBids");
                }
            }
        }
    }
}