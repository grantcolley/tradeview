using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Converters
{
    public class PercentageChangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                      object parameter, CultureInfo culture)
        {
            if (value == null
                || String.IsNullOrEmpty(value.ToString())
                || !(value is decimal))
            {
                // grey
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF808080"));
            }

            var posnegIndicator = (decimal)value;

            if (posnegIndicator > 0)
            {
                // green
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF008000"));
            }

            if (posnegIndicator < 0)
            {
                // medium violet red
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC71585"));
            }

            // grey
            return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF808080"));
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
