using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using LiveCharts;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public interface ITradeHelper
    {
        void CreateLocalTradeList<T>(
            Symbol symbol,
            IEnumerable<ITrade> tradesUpdate,
            int tradesDisplayCount,
            int tradesChartDisplayCount,
            int tradeLimit,
            out List<T> trades,
            out ChartValues<T> tradesChart) where T : TradeBase, new();

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
