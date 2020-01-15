using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using LiveCharts;
using System;
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
            int tradeLimit) where T : Trade, new();

        ChartValues<T> CreateLocalChartTrades<T>(
            IEnumerable<ITrade> tradesUpdate,
            Func<ITrade, int, int, T> createNewTrade,
            int tradesChartDisplayCount,
            int pricePrecision,
            int quantityPrecision) where T : Trade, new();

        void UpdateTrades<T>(
            Symbol symbol,
            IEnumerable<ITrade> tradesUpdate,
            List<T> currentTrades,
            int tradesDisplayCount,
            int tradesChartDisplayCount,
            out List<T> trades,
            ref ChartValues<T> tradesChart) where T : Trade, new();

        void UpdateLocalChartTrades<T>(
            IEnumerable<ITrade> tradesUpdate,
            Func<ITrade, int, int, T> createNewTrade,
            DateTime seedTime,
            long seedId,
            int tradesChartDisplayCount,
            int pricePrecision,
            int quantityPrecision,
            ref ChartValues<T> tradesChart) where T : Trade, new();
    }
}
