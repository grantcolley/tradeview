using System.Collections.Generic;
using System.Linq;
using DevelopmentInProgress.MarketView.Interface.Extensions;
using DevelopmentInProgress.Wpf.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Model;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.Common.Helpers
{
    public class BinanceOrderBookHelper : IOrderBookHelper
    {
        public OrderBook CreateLocalOrderBookReplayCache(Symbol symbol, Interface.OrderBook orderBook, int orderBookCount)
        {
            return new OrderBook
            {
                Symbol = orderBook.Symbol,
                BaseSymbol = symbol.BaseAsset.Symbol,
                QuoteSymbol = symbol.QuoteAsset.Symbol
            };
        }

        public void GetBidsAndAsks(Interface.OrderBook orderBook, int pricePrecision, int quantityPrecision, 
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
            bids = bids.Take(chartDisplayCount).ToList();
            chartBids = bids.Reverse<OrderBookPriceLevel>().ToList();

            // Create the aggregated bids and asks for the aggregated bid and ask chart.
            aggregatedAsks = chartAsks.GetAggregatedList();
            var aggBids = bids.GetAggregatedList();
            aggregatedBids = aggBids.Reverse<OrderBookPriceLevel>().ToList();
        }
    }
}
