using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DevelopmentInProgress.Wpf.Common.Converters
{
    public class LastPriceChangeDirectionColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value == null
                || String.IsNullOrEmpty(value.ToString())
                || !(value is int))
            {
                // neutral
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFBABABA"));
            }

            var posnegIndicator = (int)value;
            
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

            // neutral
            return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFBABABA"));
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}