using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        public override async Task<LocalTradeListResult<T>> CreateLocalTradeList<T>(Symbol symbol, IEnumerable<ITrade> tradesUpdate, int tradesDisplayCount, int tradesChartDisplayCount, int tradeLimit)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var snapShot = await kucoinExchangeApi.GetTradesAsync(symbol.ExchangeSymbol, tradeLimit, cancellationTokenSource.Token);

            return await base.CreateLocalTradeList<T>(symbol, snapShot, tradesDisplayCount, tradesChartDisplayCount, tradeLimit);
        }
    }
}
