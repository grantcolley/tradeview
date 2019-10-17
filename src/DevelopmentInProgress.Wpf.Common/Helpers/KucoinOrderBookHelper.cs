using System.Collections.Generic;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.Wpf.Common.Model;
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

        public OrderBook CreateLocalOrderBook(Symbol symbol, Interface.OrderBook orderBook)
        {
            throw new System.NotImplementedException();
        }

        public void GetBidsAndAsks(Interface.OrderBook orderBook, int pricePrecision, int quantityPrecision, 
            int orderBookCount, int listDisplayCount, int chartDisplayCount, 
            out List<OrderBookPriceLevel> topAsks, out List<OrderBookPriceLevel> topBids, 
            out List<OrderBookPriceLevel> chartAsks, out List<OrderBookPriceLevel> chartBids, 
            out List<OrderBookPriceLevel> aggregatedAsks, out List<OrderBookPriceLevel> aggregatedBids)
        {
            throw new System.NotImplementedException();
        }
    }
}
