using DevelopmentInProgress.Wpf.Common.Model;
using System.Collections.Generic;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.Common.Helpers
{
    public interface IOrderBookHelper
    {
        OrderBook CreateLocalOrderBook(Symbol symbol, Interface.OrderBook orderBook, int OrderBookCount);

        void GetBidsAndAsks(Interface.OrderBook orderBook, int pricePrecision, int quantityPrecision,
            int orderBookCount, int listDisplayCount, int chartDisplayCount,
            out List<OrderBookPriceLevel> topAsks, out List<OrderBookPriceLevel> topBids,
            out List<OrderBookPriceLevel> chartAsks, out List<OrderBookPriceLevel> chartBids,
            out List<OrderBookPriceLevel> aggregatedAsks, out List<OrderBookPriceLevel> aggregatedBids);
    }
}