using System;
using System.Globalization;
using System.Windows.Data;

namespace DevelopmentInProgress.Strategy.MovingAverage.Wpf.Converter
{
    public class InvertedGridRowColumnSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null
                || !(value is bool))
            {
                return 1;
            }

            var showCandlesticks = (bool)value;

            if (showCandlesticks)
            {
                return 2;
            }

            return 1;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
