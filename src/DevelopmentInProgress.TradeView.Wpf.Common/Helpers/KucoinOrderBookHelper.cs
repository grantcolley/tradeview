using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Core.Extensions;
using DevelopmentInProgress.TradeView.Core.Interfaces;
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

        public async Task<OrderBook> CreateLocalOrderBook(Symbol symbol, Core.Model.OrderBook orderBook, int listDisplayCount, int chartDisplayCount)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (orderBook == null)
            {
                throw new ArgumentNullException(nameof(orderBook));
            }

            var cancellationTokenSource = new CancellationTokenSource();

            var snapShot = await kucoinExchangeApi.GetOrderBookAsync(symbol.ExchangeSymbol, 100, cancellationTokenSource.Token).ConfigureAwait(false);

            // Order by price: bids (ASC) and asks (ASC)
            // Discard those that we are not interested in displaying on the screen.
            var snapShotAsks = new List<Core.Model.OrderBookPriceLevel>(snapShot.Asks.OrderBy(a => a.Price).ToList());
            var snapShotBids = new List<Core.Model.OrderBookPriceLevel>(snapShot.Bids.OrderBy(b => b.Price).ToList());

            long latestSquence = snapShot.LastUpdateId;

            ReplayPriceLevels(snapShotAsks, orderBook.Asks, snapShot.LastUpdateId, ref latestSquence);
            ReplayPriceLevels(snapShotBids, orderBook.Bids, snapShot.LastUpdateId, ref latestSquence);
            
            var pricePrecision = symbol.PricePrecision;
            var quantityPrecision = symbol.QuantityPrecision;
            
            var asks = snapShotAsks.Select(ask => new OrderBookPriceLevel
            {
                Price = ask.Price.Trim(pricePrecision),
                Quantity = ask.Quantity.Trim(quantityPrecision)
            }).ToList();

            var bids = snapShotBids.Select(bid => new OrderBookPriceLevel
            {
                Price = bid.Price.Trim(pricePrecision),
                Quantity = bid.Quantity.Trim(quantityPrecision)
            }).ToList();

            var topAsk = asks.First();
            var topBid = bids.First();

            decimal bidAskSpread;

            if (topAsk.Price != 0)
            {
                bidAskSpread = Math.Round(((topAsk.Price - topBid.Price) / topAsk.Price) * 100, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                bidAskSpread = 0.00m;
            }

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

            var newOrderBook = new OrderBook
            {
                LastUpdateId = latestSquence,
                Symbol = orderBook.Symbol,
                BaseSymbol = symbol.BaseAsset.Symbol,
                QuoteSymbol = symbol.QuoteAsset.Symbol,
                BidAskSpread = bidAskSpread
            };

            newOrderBook.Asks.AddRange(snapShotAsks);
            newOrderBook.Bids.AddRange(snapShotBids);
            newOrderBook.TopAsks.AddRange(topAsks);
            newOrderBook.TopBids.AddRange(topBids);
            newOrderBook.ChartAsks.AddRange(new ChartValues<OrderBookPriceLevel>(chartAsks));
            newOrderBook.ChartBids.AddRange(new ChartValues<OrderBookPriceLevel>(chartBids));
            newOrderBook.ChartAggregatedAsks.AddRange(new ChartValues<OrderBookPriceLevel>(aggregatedAsks));
            newOrderBook.ChartAggregatedBids.AddRange(new ChartValues<OrderBookPriceLevel>(aggregatedBids));

            return newOrderBook;
        }

        public void UpdateLocalOrderBook(OrderBook orderBook, Core.Model.OrderBook updateOrderBook,
            int pricePrecision, int quantityPrecision, int listDisplayCount, int chartDisplayCount)
        {
            if (orderBook == null)
            {
                throw new ArgumentNullException(nameof(orderBook));
            }

            if (updateOrderBook == null)
            {
                throw new ArgumentNullException(nameof(updateOrderBook));
            }

            long latestSquence = orderBook.LastUpdateId;

            ReplayPriceLevels(orderBook.Asks, updateOrderBook.Asks, orderBook.LastUpdateId, ref latestSquence);
            ReplayPriceLevels(orderBook.Bids, updateOrderBook.Bids, orderBook.LastUpdateId, ref latestSquence);

            orderBook.LastUpdateId = latestSquence;

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

            var topAsk = asks.First();
            var topBid = bids.First();

            if (topAsk.Price != 0)
            {
                orderBook.BidAskSpread = Math.Round(((topAsk.Price - topBid.Price) / topAsk.Price), 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                orderBook.BidAskSpread = 0.00m;
            }

            // Take the top bids and asks for the order book bid and ask lists and order descending.
            orderBook.TopAsks.Clear();
            orderBook.TopBids.Clear();
            orderBook.TopAsks.AddRange(asks.Take(listDisplayCount).OrderByDescending(a => a.Price).ToList());
            orderBook.TopBids.AddRange(bids.OrderByDescending(b => b.Price).Take(listDisplayCount).ToList());

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

        private static void ReplayPriceLevels(
            List<Core.Model.OrderBookPriceLevel> orderBookPriceLevels, 
            IEnumerable<Core.Model.OrderBookPriceLevel> playBackPriceLevels, 
            long orderBookSequence, 
            ref long latestSquence)
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
                }
            }
        }
        
        private static List<OrderBookPriceLevel> GetAggregatedAsks(List<OrderBookPriceLevel> orders)
        {
            var count = orders.Count;

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

        private static List<OrderBookPriceLevel> GetAggregatedBids(List<OrderBookPriceLevel> orders)
        {
            var index = orders.Count - 1;

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

        private static void UpdateChartValues(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl)
        {
            var count = cv.Count;

            if (count != pl.Count)
            {
                throw new IndexOutOfRangeException();
            }

            for (int i = 0; i < count; i++)
            {
                cv[i].Price = pl[i].Price;
                cv[i].Quantity = pl[i].Quantity;
            }
        }
    }
}
