using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Core.Extensions;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using LiveCharts;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public class BinanceOrderBookHelper : IOrderBookHelper
    {
        public Task<OrderBook> CreateLocalOrderBook(Symbol symbol, Core.Model.OrderBook orderBook, int listDisplayCount, int chartDisplayCount)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            if (orderBook == null)
            {
                throw new ArgumentNullException(nameof(orderBook));
            }

            var tcs = new TaskCompletionSource<OrderBook>();

            try
            {
                List<OrderBookPriceLevel> topAsks;
                List<OrderBookPriceLevel> topBids;
                List<OrderBookPriceLevel> chartAsks;
                List<OrderBookPriceLevel> chartBids;
                List<OrderBookPriceLevel> aggregatedAsks;
                List<OrderBookPriceLevel> aggregatedBids;
                decimal bidAskSpread;

                GetBidsAndAsks(orderBook, symbol.PricePrecision, symbol.QuantityPrecision, listDisplayCount, chartDisplayCount,
                    out topAsks, out topBids, out chartAsks, out chartBids, out aggregatedAsks, out aggregatedBids, out bidAskSpread);

                var newOrderBook = new OrderBook
                {
                    LastUpdateId = orderBook.LastUpdateId,
                    Symbol = orderBook.Symbol,
                    BaseSymbol = symbol.BaseAsset.Symbol,
                    QuoteSymbol = symbol.QuoteAsset.Symbol,
                    BidAskSpread = bidAskSpread
                };

                newOrderBook.TopAsks.AddRange(topAsks);
                newOrderBook.TopBids.AddRange(topBids);
                newOrderBook.ChartAsks.AddRange(new ChartValues<OrderBookPriceLevel>(chartAsks));
                newOrderBook.ChartBids.AddRange(new ChartValues<OrderBookPriceLevel>(chartBids));
                newOrderBook.ChartAggregatedAsks.AddRange(new ChartValues<OrderBookPriceLevel>(aggregatedAsks));
                newOrderBook.ChartAggregatedBids.AddRange(new ChartValues<OrderBookPriceLevel>(aggregatedBids));

                tcs.SetResult(newOrderBook);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        public void UpdateLocalOrderBook(OrderBook orderBook, Core.Model.OrderBook updateOrderBook,
            int pricePrecision, int quantityPrecision, int listDisplayCount, int chartDisplayCount)
        {
            if (updateOrderBook == null)
            {
                throw new ArgumentNullException(nameof(updateOrderBook));
            }

            if (orderBook == null)
            {
                throw new ArgumentNullException(nameof(orderBook));
            }

            orderBook.LastUpdateId = updateOrderBook.LastUpdateId;

            List<OrderBookPriceLevel> topAsks;
            List<OrderBookPriceLevel> topBids;
            List<OrderBookPriceLevel> chartAsks;
            List<OrderBookPriceLevel> chartBids;
            List<OrderBookPriceLevel> aggregatedAsks;
            List<OrderBookPriceLevel> aggregatedBids;
            decimal bidAskSpread;

            GetBidsAndAsks(updateOrderBook, pricePrecision, quantityPrecision, listDisplayCount, chartDisplayCount,
                out topAsks, out topBids, out chartAsks, out chartBids, out aggregatedAsks, out aggregatedBids, out bidAskSpread);

            // Create new instances of the top 
            // bids and asks, reversing the asks
            orderBook.TopAsks.Clear();
            orderBook.TopBids.Clear();
            orderBook.TopAsks.AddRange(topAsks);
            orderBook.TopBids.AddRange(topBids);
            orderBook.BidAskSpread = bidAskSpread;

            // Update the existing orderbook chart
            // bids and asks, reversing the bids.
            UpdateChartValues(orderBook.ChartAsks, chartAsks);
            UpdateChartValues(orderBook.ChartBids, chartBids);
            UpdateChartValues(orderBook.ChartAggregatedAsks, aggregatedAsks);
            UpdateChartValues(orderBook.ChartAggregatedBids, aggregatedBids);
        }

        private void GetBidsAndAsks(Core.Model.OrderBook orderBook, int pricePrecision, int quantityPrecision, 
            int listDisplayCount, int chartDisplayCount, 
            out List<OrderBookPriceLevel> topAsks, out List<OrderBookPriceLevel> topBids, 
            out List<OrderBookPriceLevel> chartAsks, out List<OrderBookPriceLevel> chartBids,
            out List<OrderBookPriceLevel> aggregatedAsks, out List<OrderBookPriceLevel> aggregatedBids,
            out decimal bidAskSpread)
        {
            var orderBookCount = chartDisplayCount > listDisplayCount ? chartDisplayCount : listDisplayCount;

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

            var topAsk = asks.First();
            var topBid = bids.First();
            
            if (topAsk.Price != 0)
            {
                bidAskSpread = Math.Round(((topAsk.Price - topBid.Price) / topAsk.Price)*100, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                bidAskSpread = 0.00m;
            }

            // Take the top bids and asks for the order book bid and ask lists and order descending.
            topAsks = asks.Take(listDisplayCount).Reverse().ToList();
            topBids = bids.Take(listDisplayCount).ToList();

            // Take the bid and aks to display in the the order book chart.
            chartAsks = asks.Take(chartDisplayCount).ToList();
            var tempbids = bids.Take(chartDisplayCount).ToList();
            chartBids = tempbids.Reverse<OrderBookPriceLevel>().ToList();

            // Create the aggregated bids and asks for the aggregated bid and ask chart.
            aggregatedAsks = GetAggregatedList(asks);
            var aggBids = GetAggregatedList(bids);
            aggregatedBids = aggBids.Reverse<OrderBookPriceLevel>().ToList();
        }

        private List<OrderBookPriceLevel> GetAggregatedList(List<OrderBookPriceLevel> orders)
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

        private void UpdateChartValues(ChartValues<OrderBookPriceLevel> cv, List<OrderBookPriceLevel> pl)
        {
            var count = cv.Count;

            if (count != pl.Count)
            {
                throw new IndexOutOfRangeException();
            }

            for(int i = 0; i <  count; i++)
            {
                cv[i].Price = pl[i].Price;
                cv[i].Quantity = pl[i].Quantity;
            }
        }
    }
}
