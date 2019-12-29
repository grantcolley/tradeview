using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies.Converters
{
    public class IsBuyerBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value == null
                || !(value is bool))
            {
                return null;
            }

            var isBuyer = (bool)value;

            if (isBuyer)
            {
                // green
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF008000"));
            }
            else 
            {
                // medium violet red
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC71585"));
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
