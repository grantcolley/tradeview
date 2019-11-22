using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public interface IOrderBookHelper
    {
        OrderBook CreateLocalOrderBook(Symbol symbol, Interface.Model.OrderBook orderBook, int listDisplayCount, int chartDisplayCount);

        void UpdateLocalOrderBook(OrderBook orderBook, Interface.Model.OrderBook updateOrderBook,
            int pricePrecision, int quantityPrecision, int listDisplayCount, int chartDisplayCount);
    }
}