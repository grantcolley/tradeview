using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DevelopmentInProgress.Wpf.MarketView.Converters
{
    /// <summary>
    /// Converts the message type to the image to display for the message.
    /// </summary>
    public sealed class BoolToLoginConverter : IValueConverter
    {
        private ResourceDictionary resourceDictionary;

        public ResourceDictionary ResourceDictionary
        {
            get { return resourceDictionary; }
            set { resourceDictionary = value; }
        }

        /// <summary>
        /// Converts the value to the converted type.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture information.</param>
        /// <returns>A converted type.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null
                || !(value is bool))
            {
                return null;
            }

            try
            {
                if ((bool)value == true)
                {
                    return ResourceDictionary["loggedin"];
                }

                return ResourceDictionary["login"];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts the value back to the converted type.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture information.</param>
        /// <returns>A converted type.</returns>
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
