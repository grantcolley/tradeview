using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentInProgress.Wpf.MarketView.Model
{
    public class OrderBook : EntityBase
    {
        private string baseSymbol;
        private string quoteSymbol;
        private OrderBookTop top;
        private List<OrderBookPriceLevel> asks;
        private List<OrderBookPriceLevel> bids;
        private ChartValues<OrderBookPriceLevel> chartAsks;
        private ChartValues<OrderBookPriceLevel> chartBids;

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

        public List<OrderBookPriceLevel> Asks
        {
            get { return asks; }
            set
            {
                if (asks != value)
                {
                    asks = value;
                    ChartAsks = new ChartValues<OrderBookPriceLevel>(asks.OrderBy(a => a.Price));
                    OnPropertyChanged("TopAsks");
                }
            }
        }

        public List<OrderBookPriceLevel> TopAsks
        {
            get { return asks?.Skip(asks.Count() - 10).ToList(); }
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

        public List<OrderBookPriceLevel> Bids
        {
            get { return bids; }
            set
            {
                if (bids != value)
                {
                    bids = value;
                    ChartBids = new ChartValues<OrderBookPriceLevel>(bids);
                    OnPropertyChanged("TopBids");                    
                }
            }
        }

        public List<OrderBookPriceLevel> TopBids
        {
            get { return bids?.Take(10).ToList(); ; }
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
    }
}