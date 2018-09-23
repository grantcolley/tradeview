using System;

namespace DevelopmentInProgress.Wpf.Common.Chart
{
    public interface IChartHelper
    {
        Func<double, string> TimeFormatter { get; }

        Func<double, string> PriceFormatter { get; }
    }
}
