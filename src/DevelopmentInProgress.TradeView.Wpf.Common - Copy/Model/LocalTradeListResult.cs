using LiveCharts;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class LocalTradeListResult<T>
    {
        public List<T> Trades { get; set; }
        public ChartValues<T> TradesChart { get; set; }
    }
}
