using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using LiveCharts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public interface ITradeHelper
    {
        Task<LocalTradeListResult<T>> CreateLocalTradeList<T>(
            Symbol symbol,
            IEnumerable<ITrade> tradesUpdate,
            int tradesDisplayCount,
            int tradesChartDisplayCount,
            int tradeLimit) where T : TradeBase, new();

        void UpdateTrades<T>(
            Symbol symbol,
            IEnumerable<ITrade> tradesUpdate,
            List<T> currentTrades,
            int tradesDisplayCount,
            int tradesChartDisplayCount,
            out List<T> trades,
            ref ChartValues<T> tradesChart) where T : TradeBase, new();
    }
}
