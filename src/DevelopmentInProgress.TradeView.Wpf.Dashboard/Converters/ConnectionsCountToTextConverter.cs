using DevelopmentInProgress.TradeView.Wpf.Dashboard.Model;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.Converters
{
    public class ConnectionsCountToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var connections = value as ObservableCollection<Connection>;
            if (value == null)
            {
                return "(0)";
            }

            return $"({connections.Count})";
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
