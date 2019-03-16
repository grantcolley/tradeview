using DevelopmentInProgress.Wpf.Common.Model;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentInProgress.Wpf.Common.Extensions
{
    public static class OrderBookExtensions
    {
        public static List<OrderBookPriceLevel> GetAggregatedList(this List<OrderBookPriceLevel> orders)
        {
            var count = orders.Count();

            var aggregatedList = orders.Select(p => new OrderBookPriceLevel { Price = p.Price, Quantity = p.Quantity }).ToList();

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    aggregatedList[i].Quantity = aggregatedList[i].Quantity + aggregatedList[i - 1].Quantity;
                }
            }

            return aggregatedList;
        }

        public static void UpdateChartBids(this OrderBook orderBook, List<OrderBookPriceLevel> pl)
        {
            RemoveOldPrices(orderBook.ChartBids, pl);

            UpdateMatchingPrices(orderBook.ChartBids, pl);

            AddNewPrices(orderBook.ChartBids, pl);
        }

        public static void UpdateChartAggregateBids(this OrderBook orderBook, List<OrderBookPriceLevel> pl)
        {
            RemoveOldPrices(orderBook.ChartAggregatedBids, pl);

            UpdateMatchingPrices(orderBook.ChartAggregatedBids, pl);

            AddNewPrices(orderBook.ChartAggregatedBids, pl);
        }

        public static void UpdateChartAsks(this OrderBook orderBook, List<OrderBookPriceLevel> pl)
        {
            RemoveOldPrices(orderBook.ChartAsks, pl);

            UpdateMatchingPrices(orderBook.ChartAsks, pl);

            AddNewPrices(orderBook.ChartAsks, pl);
        }

        public static void UpdateChartAggregateAsks(this OrderBook orderBook, List<OrderBookPriceLevel> pl)
        {
            RemoveOldPrices(orderBook.ChartAggregatedAsks, pl);

            UpdateMatchingPrices(orderBook.ChartAggregatedAsks, pl);

            AddNewPrices(orderBook.ChartAggregatedAsks, pl);
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

        private static void AddNewPrices(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl)
        {
            var newPoints = pl.Where(p => !cv.Any(v => v.Price == p.Price)).ToList();

            var newPointsCount = newPoints.Count;

            if(newPointsCount.Equals(0))
            {
                return;
            }

            var chartValueCount = cv.Count;

            int currentNewPoint = 0;

            for (int i = 0; i < chartValueCount; i++)
            {
                if (newPoints[currentNewPoint].Price < cv[i].Price)
                {
                    cv.Insert(i, newPoints[currentNewPoint]);

                    // Increments
                    currentNewPoint++;  // position in new points list
                    chartValueCount++;  // number of items in the cv list after the insert
                }

                if (currentNewPoint > (newPointsCount - 1))
                {
                    break;
                }

                if (i == chartValueCount - 1)
                {
                    if (currentNewPoint < newPointsCount)
                    {
                        var appendNewPoints = newPoints.Skip(currentNewPoint).ToList();
                        cv.AddRange(appendNewPoints);
                    }
                }
            }
        }
    }
}
