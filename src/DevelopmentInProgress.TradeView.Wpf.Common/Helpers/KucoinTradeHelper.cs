using System.Collections.Generic;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using LiveCharts;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public class KucoinTradeHelper : TradeHelperBase
    {
        private readonly IExchangeApi kucoinExchangeApi;

        public KucoinTradeHelper(IExchangeApi kucoinExchangeApi)
        {
            this.kucoinExchangeApi = kucoinExchangeApi;
        }

        public override void CreateLocalTradeList<T>(IEnumerable<ITrade> tradesUpdate, int pricePrecision, int quantityPrecision, int tradesDisplayCount, int tradesChartDisplayCount, out List<T> trades, out ChartValues<T> tradesChart)
        {
            // Get trades and replay trade update then create local trades list

            base.CreateLocalTradeList(tradesUpdate, pricePrecision, quantityPrecision, tradesDisplayCount, tradesChartDisplayCount, out trades, out tradesChart);
        }
    }
}
