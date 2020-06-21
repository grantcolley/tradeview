using System;
using System.Globalization;
using System.Windows.Data;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.Converters
{
    public class ItemsCountToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int v)
            {
                return $"({v})";
            }

            return "(0)";
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
