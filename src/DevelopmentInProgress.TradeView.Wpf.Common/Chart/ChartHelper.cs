using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Globalization;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Chart
{
    public class ChartHelper : IChartHelper
    {
        public ChartHelper()
        {
            var tradeMapper = Mappers.Xy<Trade>()
                .X(model => model.Time.Ticks)
                .Y(model => Convert.ToDouble(model.Price));

            Charting.For<Trade>(tradeMapper);

            var timeValuePointMapper = Mappers.Xy<TimeValuePoint>()
                .X(model => model.X.Ticks)
                .Y(model => Convert.ToDouble(model.Y));

            Charting.For<TimeValuePoint>(timeValuePointMapper);

            var candlestickMapper = Mappers.Financial<Candlestick>()
                .X((model, index) => index)
                .Open(model => Convert.ToDouble(model.Open))
                .High(model => Convert.ToDouble(model.High))
                .Low(model => Convert.ToDouble(model.Low))
                .Close(model => Convert.ToDouble(model.Close));

            Charting.For<Candlestick>(candlestickMapper, SeriesOrientation.Horizontal);
        }

        public Func<double, string> TimeFormatter => value => new DateTime((long)value).ToString("H:mm:ss", CultureInfo.InvariantCulture);

        public Func<double, string> PriceFormatter => value => value.ToString("0.00000000", CultureInfo.InvariantCulture);

        public Func<double, string> PercentageFormatter => value => Math.Round(value, 2).ToString(CultureInfo.InvariantCulture);
    }
}
