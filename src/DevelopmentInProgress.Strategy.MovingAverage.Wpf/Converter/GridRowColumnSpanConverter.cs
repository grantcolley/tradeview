using System;
using System.Globalization;
using System.Windows.Data;

namespace DevelopmentInProgress.Strategy.MovingAverage.Wpf.Converter
{
    public class GridRowColumnSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null
                || !(value is bool))
            {
                return 3;
            }

            var showCandlesticks = (bool)value;

            if (showCandlesticks)
            {
                return 1;
            }

            return 3;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
