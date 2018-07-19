using DevelopmentInProgress.Wpf.Common.Model;
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

        public void Update(List<OrderBookPriceLevel> asks, List<OrderBookPriceLevel> bids)
        {
            this.asks = asks.OrderByDescending(a => a.Price).ToList();
            this.bids = bids.OrderByDescending(b => b.Price).ToList();

            if (ChartAsks.Any())
            {
                UpdateChartValues(ChartAsks, asks, true);
            }
            else
            {
                ChartAsks = new ChartValues<OrderBookPriceLevel>(asks.OrderBy(a => a.Price));
            }

            if (ChartBids.Any())
            {
                UpdateChartValues(ChartBids, bids, false);
            }
            else
            {
                ChartBids = new ChartValues<OrderBookPriceLevel>(bids.OrderByDescending(b => b.Price));
            }

            var aggregatedAsks = GetAggregatedList(asks.OrderBy(a => a.Price).ToList());
            if (ChartAggregatedAsks.Any())
            {
                UpdateChartValues(ChartAggregatedAsks, aggregatedAsks, true);
            }
            else
            {
                ChartAggregatedAsks = new ChartValues<OrderBookPriceLevel>(aggregatedAsks.OrderBy(a => a.Price));
            }

            var aggregatedBids = GetAggregatedList(bids.OrderByDescending(a => a.Price).ToList());
            if (ChartAggregatedBids.Any())
            {
                UpdateChartValues(ChartAggregatedBids, aggregatedBids, false);
            }
            else
            {
                ChartAggregatedBids = new ChartValues<OrderBookPriceLevel>(aggregatedBids.OrderByDescending(b => b.Price));
            }

            OnPropertyChanged("TopAsks");
            OnPropertyChanged("TopBids");
        }

        public void UpdateChartValues(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl, bool isAsk)
        {
            Func<OrderBookPriceLevel, OrderBookPriceLevel, OrderBookPriceLevel> f = ((p, n) =>
            {
                p.Quantity = n.Quantity;
                return p;
            });

            var removePoints = cv.Where(v => !pl.Any(p => p.Price == v.Price)).ToList();
            foreach (var point in removePoints)
            {
                cv.Remove(point);
            }

            (from v in cv
             join p in pl
             on v.Price equals p.Price
             select f(v, p)).ToList();

            var addPoints = pl.Where(p => !cv.Any(v => v.Price == p.Price)).ToList();
            var appendRange = new List<OrderBookPriceLevel>();

            foreach (var point in addPoints)
            {
                for (int i = 0; i < cv.Count; i++)
                {
                    if (isAsk)
                    {
                        if (point.Price < cv[i].Price)
                        {
                            cv.Insert(i, point);
                            break;
                        }
                    }
                    else
                    {
                        if (point.Price > cv[i].Price)
                        {
                            cv.Insert(i, point);
                            break;
                        }
                    }

                    if (i == cv.Count - 1)
                    {
                        appendRange.Add(point);
                    }
                }
            }

            if (appendRange.Any())
            {
                if (isAsk)
                {
                    cv.AddRange(appendRange.OrderBy(p => p.Price));
                }
                else
                {
                    cv.AddRange(appendRange.OrderByDescending(p => p.Price));
                }
            }
        }

        public List<OrderBookPriceLevel> GetAggregatedList(List<OrderBookPriceLevel> pl)
        {
            var aggregatedList = pl.Select(p => new OrderBookPriceLevel { Price = p.Price, Quantity = p.Quantity }).ToList();

            for(int i = 0; i < pl.Count; i++)
            {
                if (i > 0)
                {
                    aggregatedList[i].Quantity = aggregatedList[i].Quantity + aggregatedList[i - 1].Quantity;
                }
            }

            return aggregatedList;
        }
    }
}