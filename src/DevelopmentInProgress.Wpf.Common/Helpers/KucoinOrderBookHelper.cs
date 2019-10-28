using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevelopmentInProgress.MarketView.Interface.Extensions;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.Wpf.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Model;
using LiveCharts;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.Common.Helpers
{
    public class KucoinOrderBookHelper : IOrderBookHelper
    {
        private IExchangeApi kucoinExchangeApi;

        public KucoinOrderBookHelper(IExchangeApi kucoinExchangeApi)
        {
            this.kucoinExchangeApi = kucoinExchangeApi;
        }

        public OrderBook CreateLocalOrderBook(Symbol symbol, Interface.OrderBook orderBook, int listDisplayCount, int chartDisplayCount)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var snapShot = kucoinExchangeApi.GetOrderBookAsync(symbol.ExchangeSymbol, 0, cancellationTokenSource.Token).GetAwaiter().GetResult();

            List<Interface.OrderBookPriceLevel> snapShotAsks;
            List<Interface.OrderBookPriceLevel> snapShotBids;

            var orderBookCount = chartDisplayCount > listDisplayCount ? chartDisplayCount : listDisplayCount;

            if (snapShot.Asks.Count() > orderBookCount)
            {
                snapShotAsks = new List<Interface.OrderBookPriceLevel>(snapShot.Asks);
            }
            else
            {
                snapShotAsks = new List<Interface.OrderBookPriceLevel>(snapShot.Asks);
            }

            if (snapShot.Bids.Count() > orderBookCount)
            {
                snapShotBids = new List<Interface.OrderBookPriceLevel>(snapShot.Bids);
            }
            else
            {
                snapShotBids = new List<Interface.OrderBookPriceLevel>(snapShot.Bids);
            }

            long latestSquence = snapShot.LastUpdateId;

            orderBook.Asks = ReplayPriceLevels(snapShotAsks, orderBook.Asks, snapShot.LastUpdateId, true, ref latestSquence);

            orderBook.Bids = ReplayPriceLevels(snapShotBids, orderBook.Bids, snapShot.LastUpdateId, false, ref latestSquence);

            List<OrderBookPriceLevel> topAsks;
            List<OrderBookPriceLevel> topBids;
            List<OrderBookPriceLevel> chartAsks;
            List<OrderBookPriceLevel> chartBids;
            List<OrderBookPriceLevel> aggregatedAsks;
            List<OrderBookPriceLevel> aggregatedBids;

            GetDisplayBidsAndAsks(orderBook, symbol.PricePrecision, symbol.QuantityPrecision,
                orderBookCount, listDisplayCount, chartDisplayCount,
                out topAsks, out topBids, out chartAsks, out chartBids, out aggregatedAsks, out aggregatedBids);

            return new OrderBook
            {
                LastUpdateId = latestSquence,
                Symbol = orderBook.Symbol,
                BaseSymbol = symbol.BaseAsset.Symbol,
                QuoteSymbol = symbol.QuoteAsset.Symbol,
                Asks = orderBook.Asks.ToList(),
                Bids = orderBook.Bids.ToList(),
                TopAsks = topAsks,
                TopBids = topBids,
                ChartAsks = new ChartValues<OrderBookPriceLevel>(chartAsks),
                ChartBids = new ChartValues<OrderBookPriceLevel>(chartBids),
                ChartAggregatedAsks = new ChartValues<OrderBookPriceLevel>(aggregatedAsks),
                ChartAggregatedBids = new ChartValues<OrderBookPriceLevel>(aggregatedBids)
            };
        }

        public void UpdateLocalOrderBook(OrderBook orderBook, Interface.OrderBook updateOrderBook,
            int pricePrecision, int quantityPrecision, int listDisplayCount, int chartDisplayCount)
        {
            long latestSquence = orderBook.LastUpdateId;

            orderBook.Asks = ReplayPriceLevels(orderBook.Asks, updateOrderBook.Asks, updateOrderBook.LastUpdateId, true, ref latestSquence);
            orderBook.Bids = ReplayPriceLevels(orderBook.Bids, updateOrderBook.Bids, updateOrderBook.LastUpdateId, false, ref latestSquence);

            orderBook.LastUpdateId = latestSquence;

            var orderBookCount = chartDisplayCount > listDisplayCount ? chartDisplayCount : listDisplayCount;

            var asks = orderBook.Asks.Take(orderBookCount).Select(ask => new OrderBookPriceLevel
            {
                Price = ask.Price.Trim(pricePrecision),
                Quantity = ask.Quantity.Trim(quantityPrecision)
            }).ToList();

            var bids = orderBook.Bids.Take(orderBookCount).Select(bid => new OrderBookPriceLevel
            {
                Price = bid.Price.Trim(pricePrecision),
                Quantity = bid.Quantity.Trim(quantityPrecision)
            }).ToList();

            orderBook.TopAsks = asks.Take(listDisplayCount).ToList();
            orderBook.TopBids = bids.Take(listDisplayCount).ToList();

            var chartAsks = asks.Take(chartDisplayCount).ToList();
            var chartBids = bids.Take(chartDisplayCount).ToList();

            var aggregateAsks = GetAggregatedList(asks).Take(chartDisplayCount).ToList();
            var aggregateBids = GetAggregatedList(bids).Take(chartDisplayCount).ToList();

            Func<decimal, decimal, bool> askPredicate = (p1, p2) => { return p1 < p2; };
            Func<decimal, decimal, bool> bidPredicate = (p1, p2) => { return p1 > p2; };

            UpdateChartValues(orderBook.ChartAsks, chartAsks, askPredicate);
            UpdateChartValues(orderBook.ChartBids, chartBids, bidPredicate);
            UpdateChartValues(orderBook.ChartAggregatedAsks, aggregateAsks, askPredicate);
            UpdateChartValues(orderBook.ChartAggregatedBids, aggregateBids, bidPredicate);
        }

        private List<Interface.OrderBookPriceLevel> ReplayPriceLevels(
            List<Interface.OrderBookPriceLevel> orderBookPriceLevels, 
            IEnumerable<Interface.OrderBookPriceLevel> playBackPriceLevels, 
            long orderBookSequence, 
            bool isAsk,
            ref long latestSquence)
        {
            var isBid = !isAsk;

            var priceLevels = playBackPriceLevels.Where(a => a.Id > orderBookSequence).ToList();

            foreach (var priceLevel in priceLevels)
            {
                var handled = false;
                var snapShotCount = orderBookPriceLevels.Count;

                if (priceLevel.Id > latestSquence)
                {
                    latestSquence = priceLevel.Id;
                }

                for (int i = 0; i < snapShotCount; i++)
                {
                    var price = priceLevel.Price;

                    if((isAsk && price < orderBookPriceLevels[i].Price)
                        || (isBid && price > orderBookPriceLevels[i].Price))
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
                }
            }

            return orderBookPriceLevels;
        }

        private void GetDisplayBidsAndAsks(Interface.OrderBook orderBook, int pricePrecision, int quantityPrecision,
            int orderBookCount, int listDisplayCount, int chartDisplayCount,
            out List<OrderBookPriceLevel> topAsks, out List<OrderBookPriceLevel> topBids,
            out List<OrderBookPriceLevel> chartAsks, out List<OrderBookPriceLevel> chartBids,
            out List<OrderBookPriceLevel> aggregatedAsks, out List<OrderBookPriceLevel> aggregatedBids)
        {
            // Order by price: bids (DESC) and asks (ASC)
            var orderedAsks = orderBook.Asks.OrderBy(a => a.Price).ToList();
            var orderedBids = orderBook.Bids.OrderByDescending(b => b.Price).ToList();

            // The OrderBookCount is the greater of the OrderBookDisplayCount OrderBookChartDisplayCount.
            // Take the asks and bids for the OrderBookCount as new instances of type OrderBookPriceLevel 
            // i.e. discard those that we are not interested in displaying on the screen.
            var asks = orderedAsks.Take(orderBookCount).Select(ask => new OrderBookPriceLevel
            {
                Price = ask.Price.Trim(pricePrecision),
                Quantity = ask.Quantity.Trim(quantityPrecision)
            }).ToList();

            var bids = orderedBids.Take(orderBookCount).Select(bid => new OrderBookPriceLevel
            {
                Price = bid.Price.Trim(pricePrecision),
                Quantity = bid.Quantity.Trim(quantityPrecision)
            }).ToList();

            // Take the top bids and asks for the order book bid and ask lists and order descending.
            topAsks = asks.Take(listDisplayCount).Reverse().ToList();
            topBids = bids.Take(listDisplayCount).ToList();

            // Take the bid and aks to display in the the order book chart.
            chartAsks = asks.Take(chartDisplayCount).ToList();
            chartBids = bids.Take(chartDisplayCount).ToList();

            // Create the aggregated bids and asks for the aggregated bid and ask chart.
            aggregatedAsks = GetAggregatedList(chartAsks);
            aggregatedBids = GetAggregatedList(chartBids);
        }

        private List<OrderBookPriceLevel> GetAggregatedList(List<OrderBookPriceLevel> orders)
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

        private void UpdateChartValues(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl, Func<decimal, decimal, bool> predicate)
        {
            RemoveOldPrices(cv, pl);

            UpdateMatchingPrices(cv, pl);

            AddNewPrices(cv, pl, predicate);
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

        private void AddNewPrices(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl, Func<decimal, decimal, bool> predicate)
        {
            var newPoints = pl.Where(p => !cv.Any(v => v.Price == p.Price && !p.Quantity.Equals(0m))).ToList();

            var newPointsCount = newPoints.Count;

            if (newPointsCount.Equals(0))
            {
                return;
            }

            var chartValueCount = cv.Count;

            int currentNewPoint = 0;

            for (int i = 0; i < chartValueCount; i++)
            {
                if (predicate(newPoints[currentNewPoint].Price, cv[i].Price))
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
