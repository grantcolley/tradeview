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

            orderBook.Asks = GetPriceLevels(snapShotAsks, orderBook.Asks, snapShot.LastUpdateId, true, orderBookCount, ref latestSquence);

            orderBook.Bids = GetPriceLevels(snapShotBids, orderBook.Bids, snapShot.LastUpdateId, false, orderBookCount, ref latestSquence);

            List<OrderBookPriceLevel> topAsks;
            List<OrderBookPriceLevel> topBids;
            List<OrderBookPriceLevel> chartAsks;
            List<OrderBookPriceLevel> chartBids;
            List<OrderBookPriceLevel> aggregatedAsks;
            List<OrderBookPriceLevel> aggregatedBids;

            GetBidsAndAsks(orderBook, symbol.PricePrecision, symbol.QuantityPrecision,
                orderBookCount, listDisplayCount, chartDisplayCount,
                out topAsks, out topBids, out chartAsks, out chartBids, out aggregatedAsks, out aggregatedBids);

            return new OrderBook
            {
                LastUpdateId = latestSquence,
                Symbol = orderBook.Symbol,
                BaseSymbol = symbol.BaseAsset.Symbol,
                QuoteSymbol = symbol.QuoteAsset.Symbol,
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
            throw new NotImplementedException();
        }

        private IEnumerable<Interface.OrderBookPriceLevel> GetPriceLevels(
            List<Interface.OrderBookPriceLevel> snapShotPriceLevels, 
            IEnumerable<Interface.OrderBookPriceLevel> playBackPriceLevels, 
            long snapShotSequence, 
            bool isAsk,
            int orderBookCount, 
            ref long latestSquence)
        {
            var isBid = !isAsk;

            var priceLevels = playBackPriceLevels.Where(a => a.Id > snapShotSequence).ToList();

            foreach (var priceLevel in priceLevels)
            {
                var handled = false;
                var snapShotCount = snapShotPriceLevels.Count;

                if (priceLevel.Id > latestSquence)
                {
                    latestSquence = priceLevel.Id;
                }

                for (int i = 0; i < snapShotCount; i++)
                {
                    var price = priceLevel.Price;

                    if((isAsk && price < snapShotPriceLevels[i].Price)
                        || (isBid && price > snapShotPriceLevels[i].Price))
                    {
                        if (priceLevel.Quantity > 0m)
                        {
                            snapShotPriceLevels.Insert(i, priceLevel);
                        }

                        handled = true;
                        break;
                    }

                    if (price == snapShotPriceLevels[i].Price)
                    {
                        if (priceLevel.Quantity.Equals(0m))
                        {
                            snapShotPriceLevels.Remove(snapShotPriceLevels[i]);
                        }
                        else
                        {
                            snapShotPriceLevels[i].Quantity = priceLevel.Quantity;
                        }

                        handled = true;
                        break;
                    }
                }

                if(handled == false)
                {
                    snapShotPriceLevels.Add(priceLevel);
                }
            }

            if (snapShotPriceLevels.Count >= orderBookCount)
            {
                return snapShotPriceLevels.Take(orderBookCount).ToList();
            }
            else
            {
                return snapShotPriceLevels;
            }
        }

        private void GetBidsAndAsks(Interface.OrderBook orderBook, int pricePrecision, int quantityPrecision,
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
            var tempBids = bids.Take(chartDisplayCount).ToList();
            chartBids = tempBids.Reverse<OrderBookPriceLevel>().ToList();

            // Create the aggregated bids and asks for the aggregated bid and ask chart.
            aggregatedAsks = GetAggregatedList(chartAsks);
            var aggBids = GetAggregatedList(chartBids);
            aggregatedBids = aggBids.Reverse<OrderBookPriceLevel>().ToList();
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
    }
}
