using DevelopmentInProgress.Wpf.Common.Model;
using LiveCharts;
using LiveCharts.Configurations;
using System;

namespace DevelopmentInProgress.Wpf.Common.Chart
{
    public class ChartHelper : IChartHelper
    {
        public ChartHelper()
        {
            var baseTradeMapper = Mappers.Xy<TradeBase>()
                .X(model => model.Time.Ticks)
                .Y(model => Convert.ToDouble(model.Price));

            Charting.For<TradeBase>(baseTradeMapper);
        }

        public Func<double, string> TimeFormatter => value => new DateTime((long)value).ToString("H:mm:ss");

        public Func<double, string> PriceFormatter => value => value.ToString("0.00000000");
    }
}
