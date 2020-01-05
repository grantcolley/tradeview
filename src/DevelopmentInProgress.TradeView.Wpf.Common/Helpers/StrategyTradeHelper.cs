using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public class StrategyTradeHelper : TradeHelperBase
    {
        public ChartValues<T> CreateLocalChartTrades<T>(
            IEnumerable<ITrade> tradesUpdate,
            Func<ITrade, int, int, T> createNewTrade,
            int tradesChartDisplayCount,
            int pricePrecision,
            int quantityPrecision) where T : TradeBase, new()
        {
            // Order by oldest to newest (as it will appear in the chart).
            var newTrades = (from t in tradesUpdate
                             orderby t.Time, t.Id
                             select createNewTrade(t, pricePrecision, quantityPrecision)).ToList();

            var newTradesCount = newTrades.Count;

            return GetNewChartTrades(newTrades, newTradesCount, tradesChartDisplayCount);
        }

        public void UpdateLocalChartTrades<T>(
            IEnumerable<ITrade> tradesUpdate,
            Func<ITrade, int, int, T> createNewTrade,
            DateTime seedTime,
            long seedId,
            int tradesChartDisplayCount,
            int pricePrecision,
            int quantityPrecision,
            ref ChartValues<T> tradesChart)
        {
            // Extract new trades where time and id is greater than latest available trade (seed). 
            // Order by oldest to newest (as it will appear in chart).
            var newTrades = (from t in tradesUpdate
                             where t.Time.ToLocalTime() > seedTime && t.Id > seedId
                             orderby t.Time, t.Id
                             select createNewTrade(t, pricePrecision, quantityPrecision)).ToList();

            var newTradesCount = newTrades.Count;

            UpdateChartTrades(newTrades, newTradesCount, tradesChartDisplayCount, ref tradesChart);
        }
    }
}
