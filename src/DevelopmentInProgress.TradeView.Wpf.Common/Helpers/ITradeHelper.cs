using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using LiveCharts;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public interface ITradeHelper
    {
        void CreateLocalTradeList<T>(
            IEnumerable<ITrade> tradesUpdate,
            int pricePrecision,
            int quantityPrecision,
            int tradesDisplayCount,
            int tradesChartDisplayCount,
            out List<T> trades,
            out ChartValues<T> tradesChart) where T : TradeBase, new();

        void UpdateTrades<T>(
            IEnumerable<ITrade> tradesUpdate,
            List<T> currentTrades,
            int pricePrecision,
            int quantityPrecision,
            int tradesDisplayCount,
            int tradesChartDisplayCount,
            out List<T> trades,
            ref ChartValues<T> tradesChart) where T : TradeBase, new();
    }
}
