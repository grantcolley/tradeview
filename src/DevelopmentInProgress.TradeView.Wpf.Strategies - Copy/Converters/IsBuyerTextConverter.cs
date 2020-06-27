using System;
using System.Globalization;
using System.Windows.Data;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies.Converters
{
    public class IsBuyerTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                      object parameter, CultureInfo culture)
        {
            if (value == null
                || !(value is bool))
            {
                return string.Empty;
            }

            var isBuyer = (bool)value;

            if (isBuyer)
            {
                return "B";
            }
            else
            {
                return "S";
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
