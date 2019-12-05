using System.Collections.Generic;
using System.Threading;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
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

        public override void CreateLocalTradeList<T>(Symbol symbol, IEnumerable<ITrade> tradesUpdate, int tradesDisplayCount, int tradesChartDisplayCount, int tradeLimit, out List<T> trades, out ChartValues<T> tradesChart)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var snapShot = kucoinExchangeApi.GetTradesAsync(symbol.ExchangeSymbol, tradeLimit, cancellationTokenSource.Token).GetAwaiter().GetResult();

            base.CreateLocalTradeList(symbol, snapShot, tradesDisplayCount, tradesChartDisplayCount, tradeLimit, out trades, out tradesChart);
        }
    }
}
