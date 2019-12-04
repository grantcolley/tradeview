using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevelopmentInProgress.TradeView.Interface.Extensions;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using LiveCharts;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public class KucoinOrderBookHelper : IOrderBookHelper
    {
        private readonly IExchangeApi kucoinExchangeApi;

        public KucoinOrderBookHelper(IExchangeApi kucoinExchangeApi)
        {
            this.kucoinExchangeApi = kucoinExchangeApi;
        }

        public OrderBook CreateLocalOrderBook(Symbol symbol, Interface.Model.OrderBook orderBook, int listDisplayCount, int chartDisplayCount)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var orderBookCount = chartDisplayCount > listDisplayCount ? chartDisplayCount : listDisplayCount;

            var limit = orderBookCount < 21 ? 20 : 100;

            var snapShot = kucoinExchangeApi.GetOrderBookAsync(symbol.ExchangeSymbol, limit, cancellationTokenSource.Token).GetAwaiter().GetResult();

            // Order by price: bids (ASC) and asks (ASC)
            // Discard those that we are not interested in displaying on the screen.
            var snapShotAsks = new List<Interface.Model.OrderBookPriceLevel>(snapShot.Asks.Take(orderBookCount).OrderBy(a => a.Price).ToList());
            var snapShotBids = new List<Interface.Model.OrderBookPriceLevel>(snapShot.Bids.Take(orderBookCount).OrderBy(b => b.Price).ToList());

            long latestSquence = snapShot.LastUpdateId;

            bool isUpdated = false;

            var replayedAsks = ReplayPriceLevels(snapShotAsks, orderBook.Asks, snapShot.LastUpdateId, ref latestSquence, ref isUpdated);

            var replayedBids = ReplayPriceLevels(snapShotBids, orderBook.Bids, snapShot.LastUpdateId, ref latestSquence, ref isUpdated);
            
            var pricePrecision = symbol.PricePrecision;
            var quantityPrecision = symbol.QuantityPrecision;
            
            var asks = replayedAsks.Select(ask => new OrderBookPriceLevel
            {
                Price = ask.Price.Trim(pricePrecision),
                Quantity = ask.Quantity.Trim(quantityPrecision)
            }).ToList();

            var bids = replayedBids.Select(bid => new OrderBookPriceLevel
            {
                Price = bid.Price.Trim(pricePrecision),
                Quantity = bid.Quantity.Trim(quantityPrecision)
            }).ToList();

            // Take the top bids and asks for the order book bid and ask lists and order descending.
            var topAsks = asks.Take(listDisplayCount).OrderByDescending(a => a.Price).ToList();
            var topBids = bids.OrderByDescending(b => b.Price).Take(listDisplayCount).ToList();

            var skipExcessBids = 0;
            if (bids.Count > chartDisplayCount)
            {
                skipExcessBids = bids.Count - chartDisplayCount;
            }

            // Take the bid and aks to display in the the order book chart.
            var chartAsks = asks.Take(chartDisplayCount).ToList();
            var chartBids = bids.Skip(skipExcessBids).ToList();

            // Create the aggregated bids and asks for the aggregated bid and ask chart.
            var aggregatedAsks = GetAggregatedAsks(chartAsks);
            var aggregatedBids = GetAggregatedBids(chartBids);

            return new OrderBook
            {
                LastUpdateId = latestSquence,
                Symbol = orderBook.Symbol,
                BaseSymbol = symbol.BaseAsset.Symbol,
                QuoteSymbol = symbol.QuoteAsset.Symbol,
                Asks = replayedAsks,
                Bids = replayedBids,
                TopAsks = topAsks,
                TopBids = topBids,
                ChartAsks = new ChartValues<OrderBookPriceLevel>(chartAsks),
                ChartBids = new ChartValues<OrderBookPriceLevel>(chartBids),
                ChartAggregatedAsks = new ChartValues<OrderBookPriceLevel>(aggregatedAsks),
                ChartAggregatedBids = new ChartValues<OrderBookPriceLevel>(aggregatedBids)
            };
        }

        public void UpdateLocalOrderBook(OrderBook orderBook, Interface.Model.OrderBook updateOrderBook,
            int pricePrecision, int quantityPrecision, int listDisplayCount, int chartDisplayCount)
        {
            long latestSquence = orderBook.LastUpdateId;

            if (updateOrderBook.Asks.Any(a => a.Price > 0)
                || updateOrderBook.Bids.Any(b => b.Price > 0))
            {
                var x = string.Empty;
            }

            bool isUpdated = false;

            orderBook.Asks = ReplayPriceLevels(orderBook.Asks, updateOrderBook.Asks, orderBook.LastUpdateId, ref latestSquence, ref isUpdated);
            orderBook.Bids = ReplayPriceLevels(orderBook.Bids, updateOrderBook.Bids, orderBook.LastUpdateId, ref latestSquence, ref isUpdated);

            orderBook.LastUpdateId = latestSquence;

            var orderBookCount = chartDisplayCount > listDisplayCount ? chartDisplayCount : listDisplayCount;

            var asks = orderBook.Asks.Select(ask => new OrderBookPriceLevel
            {
                Price = ask.Price.Trim(pricePrecision),
                Quantity = ask.Quantity.Trim(quantityPrecision)
            }).ToList();

            var bids = orderBook.Bids.Select(bid => new OrderBookPriceLevel
            {
                Price = bid.Price.Trim(pricePrecision),
                Quantity = bid.Quantity.Trim(quantityPrecision)
            }).ToList();

            // Take the top bids and asks for the order book bid and ask lists and order descending.
            orderBook.TopAsks = asks.Take(listDisplayCount).OrderByDescending(a => a.Price).ToList();
            orderBook.TopBids = bids.OrderByDescending(b => b.Price).Take(listDisplayCount).ToList();

            var skipExcessBids = 0;
            if (bids.Count > chartDisplayCount)
            {
                skipExcessBids = bids.Count - chartDisplayCount;
            }

            // Take the bid and aks to display in the the order book chart.
            var chartAsks = asks.Take(chartDisplayCount).ToList();
            var chartBids = bids.Skip(skipExcessBids).ToList();

            // Create the aggregated bids and asks for the aggregated bid and ask chart.
            var aggregatedAsks = GetAggregatedAsks(chartAsks);
            var aggregatedBids = GetAggregatedBids(chartBids);

            UpdateChartValues(orderBook.ChartAsks, chartAsks);
            UpdateChartValues(orderBook.ChartBids, chartBids);
            UpdateChartValues(orderBook.ChartAggregatedAsks, aggregatedAsks);
            UpdateChartValues(orderBook.ChartAggregatedBids, aggregatedBids);
        }

        private List<Interface.Model.OrderBookPriceLevel> ReplayPriceLevels(
            List<Interface.Model.OrderBookPriceLevel> orderBookPriceLevels, 
            IEnumerable<Interface.Model.OrderBookPriceLevel> playBackPriceLevels, 
            long orderBookSequence, 
            ref long latestSquence,
            ref bool isUpdated)
        {
            var priceLevels = playBackPriceLevels.Where(a => a.Id > orderBookSequence).ToList();

            foreach (var priceLevel in priceLevels)
            {
                if (priceLevel.Id > latestSquence)
                {
                    latestSquence = priceLevel.Id;
                }

                if(priceLevel.Price.Equals(0))
                {
                    continue;
                }

                var handled = false;
                var snapShotCount = orderBookPriceLevels.Count;

                for (int i = 0; i < snapShotCount; i++)
                {
                    var price = priceLevel.Price;

                    if(price < orderBookPriceLevels[i].Price)
                    {
                        if (priceLevel.Quantity > 0m)
                        {
                            orderBookPriceLevels.Insert(i, priceLevel);
                        }

                        handled = true;
                        break;
                    }

                    if (price == orderBookPriceLevels[i].Price)
                    {
                        if (priceLevel.Quantity.Equals(0m))
                        {
                            orderBookPriceLevels.Remove(orderBookPriceLevels[i]);
                        }
                        else
                        {
                            orderBookPriceLevels[i].Quantity = priceLevel.Quantity;
                        }

                        handled = true;
                        break;
                    }
                }

                if(handled == false)
                {
                    orderBookPriceLevels.Add(priceLevel);
                    handled = true;
                }

                if(isUpdated.Equals(false)
                    && handled)
                {
                    isUpdated = true;
                }
            }

            return orderBookPriceLevels;
        }
        
        private List<OrderBookPriceLevel> GetAggregatedAsks(List<OrderBookPriceLevel> orders)
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

        private List<OrderBookPriceLevel> GetAggregatedBids(List<OrderBookPriceLevel> orders)
        {
            var index = orders.Count() - 1;

            var aggregatedList = orders.Select(p => new OrderBookPriceLevel { Price = p.Price, Quantity = p.Quantity }).ToList();

            for (int i = index; i >= 0; i--)
            {
                if (i < index)
                {
                    aggregatedList[i].Quantity = aggregatedList[i].Quantity + aggregatedList[i + 1].Quantity;
                }
            }

            return aggregatedList;

        }

        private void UpdateChartValues(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl)
        {
            RemoveOldPrices(cv, pl);

            UpdateMatchingPrices(cv, pl);

            AddNewPrices(cv, pl);
        }

        private void RemoveOldPrices(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl)
        {
            var removePoints = cv.Where(v => !pl.Any(p => p.Price == v.Price)).ToList();
            foreach (var point in removePoints)
            {
                cv.Remove(point);
            }
        }

        private void UpdateMatchingPrices(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl)
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

        private void AddNewPrices(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl)
        {
            var newPoints = pl.Where(p => !cv.Any(v => v.Price == p.Price)).ToList();

            var newPointsCount = newPoints.Count;

            if (newPointsCount.Equals(0))
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
                    // No more new points to add so break out
                    break;
                }

                if (i == chartValueCount - 1)
                {
                    // We have reached the end of the chart values
                    if (currentNewPoint < newPointsCount)
                    {
                        // Add the remaining new points
                        var appendNewPoints = newPoints.Skip(currentNewPoint).ToList();
                        cv.AddRange(appendNewPoints);
                    }
                }
            }
        }
    }
}
