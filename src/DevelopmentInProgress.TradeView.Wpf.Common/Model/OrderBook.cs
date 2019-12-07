using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class OrderBook : EntityBase
    {
        private string baseSymbol;
        private string quoteSymbol;
        private List<OrderBookPriceLevel> topAsks;
        private List<OrderBookPriceLevel> topBids;
        private ChartValues<OrderBookPriceLevel> chartAsks;
        private ChartValues<OrderBookPriceLevel> chartBids;
        private ChartValues<OrderBookPriceLevel> chartAggregatedAsks;
        private ChartValues<OrderBookPriceLevel> chartAggregatedBids;

        private bool isStaged;
        private List<OrderBookPriceLevel> stageChartAsks;
        private List<OrderBookPriceLevel> stageChartBids;
        private List<OrderBookPriceLevel> stageChartAggregatedAsks;
        private List<OrderBookPriceLevel> stageChartAggregatedBids;

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

        public List<Interface.Model.OrderBookPriceLevel> Asks { get; set; }
        public List<Interface.Model.OrderBookPriceLevel> Bids { get; set; }

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

        public void StageChartValues()
        {
            stageChartAsks = ChartAsks.ToList();
            stageChartBids = ChartBids.ToList();
            stageChartAggregatedAsks = ChartAggregatedAsks.ToList();
            stageChartAggregatedBids = ChartAggregatedBids.ToList();

            ChartAsks.Clear();
            ChartBids.Clear();
            ChartAggregatedAsks.Clear();
            ChartAggregatedBids.Clear();
            isStaged = true;
        }

        public void UnstageChartValues()
        {
            if (isStaged)
            {
                ChartAsks.AddRange(stageChartAsks);
                ChartBids.AddRange(stageChartBids);
                ChartAggregatedAsks.AddRange(stageChartAggregatedAsks);
                ChartAggregatedBids.AddRange(stageChartAggregatedBids);
                isStaged = false;
            }
        }
    }
}