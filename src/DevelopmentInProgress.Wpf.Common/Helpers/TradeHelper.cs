using DevelopmentInProgress.MarketView.Interface.Extensions;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.Wpf.Common.Model;
using LiveCharts;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DevelopmentInProgress.Wpf.Common.Helpers
{
    public static class TradeHelper
    {
        public static List<T> GetNewTradesList<T>(
            IEnumerable<ITrade> tradesUpdate,
            Func<ITrade, T> create,
            int tradesDisplayCount,
            ILoggerFacade logger) where T : TradeBase, new()
        {
            List<T> trades;

            var sw = new Stopwatch();
            sw.Start();
            logger.Log($"Start GetTradesList<{typeof(T).Name}>", Category.Info, Priority.Low);

            // Order by oldest to newest (as it will appear in the chart).
            var newTrades = (from t in tradesUpdate
                             orderby t.Time, t.Id
                             select create(t)).ToList();

            var newTradesCount = newTrades.Count;

            if (newTradesCount > tradesDisplayCount)
            {
                // More new trades than the list can take, only takes the newest trades.
                var tradeBooktrades = newTrades.Skip(newTradesCount - tradesDisplayCount).ToList();

                // Order by newest to oldest (as it will appear on trade list)
                trades = new List<T>(tradeBooktrades.Reverse<T>().ToList());
            }
            else
            {
                // New trades less (or equal) the 
                // total trades to show in the trade list.
                // Order by newest to oldest (as it will appear on trade list)
                trades = new List<T>(newTrades.Reverse<T>().ToList());
            }

            sw.Stop();
            logger.Log($"End GetTradesList<{typeof(T).Name}> {sw.Elapsed}", Category.Info, Priority.Low);

            return trades;
        }

        public static ChartValues<T> GetNewTradesChart<T>(
            IEnumerable<ITrade> tradesUpdate,
            Func<ITrade, T> create,
            int tradesChartDisplayCount,
            ILoggerFacade logger) where T : TradeBase, new()
        {
            ChartValues<T> tradesChart;

            var sw = new Stopwatch();
            sw.Start();
            logger.Log($"Start GetTradesChart<{typeof(T).Name}>", Category.Info, Priority.Low);

            // Order by oldest to newest (as it will appear in the chart).
            var newTrades = (from t in tradesUpdate
                             orderby t.Time, t.Id
                             select create(t)).ToList();

            var newTradesCount = newTrades.Count;

            if (newTradesCount > tradesChartDisplayCount)
            {
                // More new trades than the chart can take, only takes the newest trades.
                var chartTrades = newTrades.Skip(newTradesCount - tradesChartDisplayCount).ToList();
                tradesChart = new ChartValues<T>(chartTrades);
            }
            else
            {
                // New trades less (or equal) the 
                // total trades to show in the chart.
                tradesChart = new ChartValues<T>(newTrades);
            }

            sw.Stop();
            logger.Log($"End GetTradesChart<{typeof(T).Name}> {sw.Elapsed}", Category.Info, Priority.Low);

            return tradesChart;
        }

        public static List<T> GetUpdatedTradesList<T>(
            IEnumerable<ITrade> tradesUpdate,
            List<T> currentTrades,
            Func<ITrade, T> create,
            int tradesDisplayCount,
            ILoggerFacade logger) where T : TradeBase, new()
        {
            List<T> trades;

            var sw = new Stopwatch();
            sw.Start();
            logger.Log($"Start GetUpdatedTradesList<{typeof(T).Name}>", Category.Info, Priority.Low);

            // Get the latest available trade - the first trade on the 
            // trade list (which is also the last trade in the chart).
            var seed = currentTrades.First();

            // Extract new trades where time and id is greater than latest available trade. 
            // Order by oldest to newest (as it will appear in chart).
            var newTrades = (from t in tradesUpdate
                             where t.Time > seed.Time && t.Id > seed.Id
                             orderby t.Time, t.Id
                             select create(t)).ToList();

            var newTradesCount = newTrades.Count;

            if (newTradesCount >= tradesDisplayCount)
            {
                // More new trades than the list can take, only take the newest
                // trades and create a new instance of the trades list.
                var tradeBooktrades = newTrades.Skip(newTradesCount - tradesDisplayCount).ToList();

                // Order by newest to oldest (as it will appear on trade list)
                trades = new List<T>(tradeBooktrades.Reverse<T>().ToList());
            }
            else
            {
                var tradesCount = currentTrades.Count;

                // Order the new trades by newest first and oldest last
                var tradeBooktrades = newTrades.Reverse<T>().ToList();

                if ((newTradesCount + tradesCount) > tradesDisplayCount)
                {
                    // Append to the new trades the balance from the existing trades to make up the trade list limit
                    var balanceTrades = currentTrades.Take(tradesDisplayCount - newTradesCount).ToList();
                    tradeBooktrades.AddRange(balanceTrades);
                }
                else
                {
                    // Simply append the existing trades to the new trades as it will fit in the trade list limit.
                    tradeBooktrades.AddRange(currentTrades);
                }

                trades = tradeBooktrades;
            }

            sw.Stop();
            logger.Log($"End GetUpdatedTradesList<{typeof(T).Name}> {sw.Elapsed}", Category.Info, Priority.Low);

            return trades;
        }

        public static void UpdateTradesChart<T>(
            IEnumerable<ITrade> tradesUpdate,
            Func<ITrade, T> create,
            int tradesChartDisplayCount,
            ILoggerFacade logger,
            ref ChartValues<T> tradesChart) where T : TradeBase, new()
        {
            var sw = new Stopwatch();
            sw.Start();
            logger.Log($"Start UpdateTradesChart<{typeof(T).Name}>", Category.Info, Priority.Low);

            // Get the latest available trade - the last trade on the trade chart.
            var seed = tradesChart.Last();

            // Extract new trades where time and id is greater than latest available trade. 
            // Order by oldest to newest (as it will appear in chart).
            var newTrades = (from t in tradesUpdate
                             where t.Time > seed.Time && t.Id > seed.Id
                             orderby t.Time, t.Id
                             select create(t)).ToList();

            var newTradesCount = newTrades.Count;
            var tradesChartCount = tradesChart.Count;

            if (tradesChartCount >= tradesChartDisplayCount)
            {
                // For each additional new trade remove the oldest then add the new trade
                for (int i = 0; i < newTradesCount; i++)
                {
                    tradesChart.RemoveAt(0);
                    tradesChart.Add(newTrades[i]);
                }
            }
            else
            {
                // Get the difference between the number of trades the chart can take and the number it currently holds.
                var chartDisplayTopUpTradesCount = tradesChartDisplayCount - tradesChartCount;

                if (newTradesCount > chartDisplayTopUpTradesCount)
                {
                    // There are more new trades than the chart can take.

                    if (chartDisplayTopUpTradesCount > 0)
                    {
                        // The top up trades can simply be added to the chart as it will take it to the total the chart can hold
                        var chartDisplayTopUpTrades = newTrades.Take(chartDisplayTopUpTradesCount).ToList();
                        tradesChart.AddRange(chartDisplayTopUpTrades);
                    }

                    for (int i = chartDisplayTopUpTradesCount; i < newTradesCount; i++)
                    {
                        // For each additional new trade remove the oldest then add the new trade
                        tradesChart.RemoveAt(0);
                        tradesChart.Add(newTrades[i]);
                    }
                }
                else
                {
                    // Simply add new trades to current list as it wont be more than the total the chart can take.
                    tradesChart.AddRange(newTrades);
                }
            }

            sw.Stop();
            logger.Log($"End UpdateTradesChart<{typeof(T).Name}> {sw.Elapsed}", Category.Info, Priority.Low);
        }

        public static void SetTrades<T>(
            IEnumerable<ITrade> tradesUpdate,
            int pricePrecision,
            int quantityPrecision,
            int tradesDisplayCount,
            int tradesChartDisplayCount,
            ILoggerFacade logger,
            out List<T> trades,
            out ChartValues<T> tradesChart) where T : TradeBase, new()
        {
            var sw = new Stopwatch();
            sw.Start();
            logger.Log($"Start SetTrades", Category.Info, Priority.Low);

            // Order by oldest to newest (as it will appear in the chart).
            var newTrades = (from t in tradesUpdate
                             orderby t.Time, t.Id
                             select new T
                             {
                                 Id = t.Id,
                                 Time = t.Time.ToLocalTime(),
                                 Symbol = t.Symbol,
                                 IsBuyerMaker = t.IsBuyerMaker,
                                 IsBestPriceMatch = t.IsBestPriceMatch,
                                 Price = t.Price.Trim(pricePrecision),
                                 Quantity = t.Quantity.Trim(quantityPrecision)
                             }).ToList();

            var newTradesCount = newTrades.Count;

            if (newTradesCount > tradesChartDisplayCount)
            {
                // More new trades than the chart can take, only takes the newest trades.
                var chartTrades = newTrades.Skip(newTradesCount - tradesChartDisplayCount).ToList();
                tradesChart = new ChartValues<T>(chartTrades);
            }
            else
            {
                // New trades less (or equal) the 
                // total trades to show in the chart.
                tradesChart = new ChartValues<T>(newTrades);
            }

            if (newTradesCount > tradesDisplayCount)
            {
                // More new trades than the list can take, only takes the newest trades.
                var tradeBooktrades = newTrades.Skip(newTradesCount - tradesDisplayCount).ToList();

                // Order by newest to oldest (as it will appear on trade list)
                trades = new List<T>(tradeBooktrades.Reverse<T>().ToList());
            }
            else
            {
                // New trades less (or equal) the 
                // total trades to show in the trade list.
                // Order by newest to oldest (as it will appear on trade list)
                trades = new List<T>(newTrades.Reverse<T>().ToList());
            }

            sw.Stop();
            logger.Log($"End SetTrades {sw.Elapsed}", Category.Info, Priority.Low);
        }
        
        public static void UpdateTrades<T>(
            IEnumerable<ITrade> tradesUpdate,
            List<T> currentTrades,
            int pricePrecision,
            int quantityPrecision,
            int tradesDisplayCount,
            int tradesChartDisplayCount,
            ILoggerFacade logger,
            out List<T> trades,
            ref ChartValues<T> tradesChart) where T : TradeBase, new()
        {
            var sw = new Stopwatch();
            sw.Start();
            logger.Log($"Start TradeUpdate", Category.Info, Priority.Low);

            // Get the latest available trade - the first trade on the 
            // trade list (which is also the last trade in the chart).
            var seed = currentTrades.First();

            // Extract new trades where time and id is greater than latest available trade. 
            // Order by oldest to newest (as it will appear in chart).
            var newTrades = (from t in tradesUpdate
                             where t.Time > seed.Time && t.Id > seed.Id
                             orderby t.Time, t.Id
                             select new T
                             {
                                 Id = t.Id,
                                 Time = t.Time.ToLocalTime(),
                                 Symbol = t.Symbol,
                                 IsBuyerMaker = t.IsBuyerMaker,
                                 IsBestPriceMatch = t.IsBestPriceMatch,
                                 Price = t.Price.Trim(pricePrecision),
                                 Quantity = t.Quantity.Trim(quantityPrecision)
                             }).ToList();

            var newTradesCount = newTrades.Count;
            var tradesChartCount = tradesChart.Count;

            if (tradesChartCount >= tradesChartDisplayCount)
            {
                // For each additional new trade remove the oldest then add the new trade
                for (int i = 0; i < newTradesCount; i++)
                {
                    tradesChart.RemoveAt(0);
                    tradesChart.Add(newTrades[i]);
                }
            }
            else
            {
                // Get the difference between the number of trades the chart can take and the number it currently holds.
                var chartDisplayTopUpTradesCount = tradesChartDisplayCount - tradesChartCount;

                if (newTradesCount > chartDisplayTopUpTradesCount)
                {
                    // There are more new trades than the chart can take.

                    if (chartDisplayTopUpTradesCount > 0)
                    {
                        // The top up trades can simply be added to the chart as it will take it to the total the chart can hold
                        var chartDisplayTopUpTrades = newTrades.Take(chartDisplayTopUpTradesCount).ToList();
                        tradesChart.AddRange(chartDisplayTopUpTrades);
                    }

                    for (int i = chartDisplayTopUpTradesCount; i < newTradesCount; i++)
                    {
                        // For each additional new trade remove the oldest then add the new trade
                        tradesChart.RemoveAt(0);
                        tradesChart.Add(newTrades[i]);
                    }
                }
                else
                {
                    // Simply add new trades to current list as it wont be more than the total the chart can take.
                    tradesChart.AddRange(newTrades);
                }
            }

            if (newTradesCount >= tradesDisplayCount)
            {
                // More new trades than the list can take, only take the newest
                // trades and create a new instance of the trades list.
                var tradeBooktrades = newTrades.Skip(newTradesCount - tradesDisplayCount).ToList();

                // Order by newest to oldest (as it will appear on trade list)
                trades = new List<T>(tradeBooktrades.Reverse<T>().ToList());
            }
            else
            {
                var tradesCount = currentTrades.Count;

                // Order the new trades by newest first and oldest last
                var tradeBooktrades = newTrades.Reverse<T>().ToList();

                if ((newTradesCount + tradesCount) > tradesDisplayCount)
                {
                    // Append to the new trades the balance from the existing trades to make up the trade list limit
                    var balanceTrades = currentTrades.Take(tradesDisplayCount - newTradesCount).ToList();
                    tradeBooktrades.AddRange(balanceTrades);
                }
                else
                {
                    // Simply append the existing trades to the new trades as it will fit in the trade list limit.
                    tradeBooktrades.AddRange(currentTrades);
                }

                trades = tradeBooktrades;
            }

            sw.Stop();
            logger.Log($"End TradeUpdate {sw.Elapsed}", Category.Info, Priority.Low);
        }
    }
}