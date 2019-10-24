using DevelopmentInProgress.Wpf.Common.Model;
using System.Collections.Generic;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.Common.Helpers
{
    public interface IOrderBookHelper
    {
        OrderBook CreateLocalOrderBook(Symbol symbol, Interface.OrderBook orderBook, int listDisplayCount, int chartDisplayCount);

        void UpdateLocalOrderBook(OrderBook orderBook, Interface.OrderBook updateOrderBook,
            int pricePrecision, int quantityPrecision, int listDisplayCount, int chartDisplayCount);
    }
}