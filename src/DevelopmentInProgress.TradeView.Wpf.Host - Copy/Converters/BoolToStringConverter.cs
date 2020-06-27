//-----------------------------------------------------------------------
// <copyright file="BoolToStringConverter.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;

namespace DevelopmentInProgress.TradeView.Wpf.Host.Converters
{
    /// <summary>
    /// Converts a boolean to a string representing true or false.
    /// </summary>
    public class BoolToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts the value to the target type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The converted type.</returns>
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value != null
                && (bool)value)
            {
                return "True";
            }

            return "False";
        }

        /// <summary>
        /// Converts the value to the target type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The converted type.</returns>
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
