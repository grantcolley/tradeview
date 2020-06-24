using System;
using System.Globalization;
using System.Windows.Data;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Converters
{
    public class ZeroToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value != null
                && value is decimal
                && (decimal)value == 0m)
            {
                return null;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}