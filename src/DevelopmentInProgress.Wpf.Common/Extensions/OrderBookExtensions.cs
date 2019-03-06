using DevelopmentInProgress.Wpf.Common.Model;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentInProgress.Wpf.Common.Extensions
{
    public static class OrderBookExtensions
    {
        public static void UpdateChartBids(this OrderBook orderBook, List<OrderBookPriceLevel> pl)
        {
            RemoveOldPrices(orderBook.ChartBids, pl);

            UpdateMatchingPrices(orderBook.ChartBids, pl);

            AddNewPrices(orderBook.ChartBids, pl, false);
        }

        public static void UpdateChartAggregateBids(this OrderBook orderBook, List<OrderBookPriceLevel> pl)
        {
            RemoveOldPrices(orderBook.ChartAggregatedBids, pl);

            UpdateMatchingPrices(orderBook.ChartAggregatedBids, pl);

            AddNewPrices(orderBook.ChartAggregatedBids, pl, false);
        }

        public static void UpdateChartAsks(this OrderBook orderBook, List<OrderBookPriceLevel> pl)
        {
            RemoveOldPrices(orderBook.ChartAsks, pl);

            UpdateMatchingPrices(orderBook.ChartAsks, pl);

            AddNewPrices(orderBook.ChartAsks, pl, true);
        }

        public static void UpdateChartAggregateAsks(this OrderBook orderBook, List<OrderBookPriceLevel> pl)
        {
            RemoveOldPrices(orderBook.ChartAggregatedAsks, pl);

            UpdateMatchingPrices(orderBook.ChartAggregatedAsks, pl);

            AddNewPrices(orderBook.ChartAggregatedAsks, pl, true);
        }

        private static void RemoveOldPrices(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl)
        {
            var removePoints = cv.Where(v => !pl.Any(p => p.Price == v.Price)).ToList();
            foreach (var point in removePoints)
            {
                cv.Remove(point);
            }
        }

        private static void UpdateMatchingPrices(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl)
        {
            Func<OrderBookPriceLevel, OrderBookPriceLevel, OrderBookPriceLevel> updateQuantity = ((v, p) =>
            {
                v.Quantity = p.Quantity;
                return v;
            });

            (from v in cv
             join p in pl
             on v.Price equals p.Price
             select updateQuantity(v, p)).ToList();
        }

        private static void AddNewPrices(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl, bool isAsks)
        {
            var newPoints = pl.Where(p => !cv.Any(v => v.Price == p.Price)).ToList();

            var newPointsCount = newPoints.Count;
            var chartValueCount = cv.Count;

            int currentNewPoint = 0;

            for (int i = 0; i < chartValueCount; i++)
            {
                if (isAsks)
                {
                    if (newPoints[currentNewPoint].Price < cv[i].Price)
                    {
                        cv.Insert(i, newPoints[currentNewPoint]);
                        currentNewPoint++;
                    }
                }
                else
                {
                    if (newPoints[currentNewPoint].Price > cv[i].Price)
                    {
                        cv.Insert(i, newPoints[currentNewPoint]);
                        currentNewPoint++;
                    }
                }

                // TODO: does this only apply to asks?
                if (i == chartValueCount - 1)
                {
                    if (currentNewPoint < newPointsCount - 1)
                    {
                        var appendNewPoints = newPoints.Skip(currentNewPoint).ToList();
                        cv.AddRange(appendNewPoints);
                    }
                }
            }
        }
    }
}
