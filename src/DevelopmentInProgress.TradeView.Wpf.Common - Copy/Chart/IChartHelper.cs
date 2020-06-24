using System;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Chart
{
    public interface IChartHelper
    {
        Func<double, string> TimeFormatter { get; }

        Func<double, string> PriceFormatter { get; }

        Func<double, string> PercentageFormatter { get; }
    }
}
