using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public interface IOrderBookHelper
    {
        Task<OrderBook> CreateLocalOrderBook(Symbol symbol, Core.Model.OrderBook orderBook, int listDisplayCount, int chartDisplayCount);

        void UpdateLocalOrderBook(OrderBook orderBook, Core.Model.OrderBook updateOrderBook,
            int pricePrecision, int quantityPrecision, int listDisplayCount, int chartDisplayCount);
    }
}