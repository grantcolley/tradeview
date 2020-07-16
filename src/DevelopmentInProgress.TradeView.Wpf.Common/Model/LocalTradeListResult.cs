using LiveCharts;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class LocalTradeListResult<T>
    {
        public LocalTradeListResult()
        {
            Trades = new List<T>();
            TradesChart = new ChartValues<T>();
        }

        public List<T> Trades { get; }
        public ChartValues<T> TradesChart { get; }
    }
}
