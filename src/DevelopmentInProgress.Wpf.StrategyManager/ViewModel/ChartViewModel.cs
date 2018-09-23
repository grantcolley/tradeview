using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Common.ViewModel;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using LiveCharts;
using System.Collections.Generic;
using System.Linq;
using DevelopmentInProgress.Wpf.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Chart;
using System;

namespace DevelopmentInProgress.Wpf.StrategyManager.ViewModel
{
    public class ChartViewModel : BaseViewModel
    {
        internal int ChartDisplayLimit => 100;

        private List<Symbol> symbols;
        private ChartValues<AggregateTrade> aggregateTradesChart;
        private object aggregateTradesLock = new object();
        private bool isLoadingTrades;
        private bool isActive;
        private bool disposed;

        public ChartViewModel(IChartHelper chartHelper)
        {
            TimeFormatter = chartHelper.TimeFormatter;
            PriceFormatter = chartHelper.PriceFormatter;

            IsActive = false;
            IsLoadingTrades = true;
        }

        public Func<double, string> TimeFormatter { get; set; }

        public Func<double, string> PriceFormatter { get; set; }

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

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    OnPropertyChanged("IsActive");
                }
            }
        }

        public List<Symbol> Symbols
        {
            get { return symbols; }
            set
            {
                if (symbols != value)
                {
                    symbols = value;
                    OnPropertyChanged("Symbols");
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

        public override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // do stuff...
            }

            disposed = true;
        }

        public void UpdateTrades(IEnumerable<Interface.AggregateTrade> trades)
        {
            if (symbols != null)
            {
                lock (aggregateTradesLock)
                {
                    IsLoadingTrades = false;

                    var trade = trades.First();

                    var symbol = Symbols.First(s => s.Name.Equals(trade.Symbol));

                    if (AggregateTradesChart == null)
                    {
                        var orderedTrades = (from t in trades
                                             orderby t.Id
                                             select new AggregateTrade
                                             {
                                                 Id = t.Id,
                                                 Time = t.Time,
                                                 Price = t.Price.Trim(symbol.PricePrecision),
                                                 Quantity = t.Quantity.Trim(symbol.QuantityPrecision),
                                                 IsBuyerMaker = t.IsBuyerMaker
                                             });

                        AggregateTradesChart = new ChartValues<AggregateTrade>(orderedTrades);
                    }
                    else
                    {
                        var maxId = AggregateTradesChart.Max(at => at.Id);
                        var orderedAggregateTrades = (from t in trades
                                                      where t.Id > maxId
                                                      orderby t.Id
                                                      select new AggregateTrade
                                                      {
                                                          Id = t.Id,
                                                          Time = t.Time,
                                                          Price = t.Price.Trim(symbol.PricePrecision),
                                                          Quantity = t.Quantity.Trim(symbol.QuantityPrecision),
                                                          IsBuyerMaker = t.IsBuyerMaker
                                                      }).ToList();

                        var newCount = orderedAggregateTrades.Count();

                        if (AggregateTradesChart.Count >= ChartDisplayLimit)
                        {
                            var oldTrades = AggregateTradesChart.Take(newCount);
                            foreach (var oldTrade in oldTrades)
                            {
                                AggregateTradesChart.Remove(oldTrade);
                            }
                        }

                        AggregateTradesChart.AddRange(orderedAggregateTrades);
                    }
                }
            }
        }
    }
}
