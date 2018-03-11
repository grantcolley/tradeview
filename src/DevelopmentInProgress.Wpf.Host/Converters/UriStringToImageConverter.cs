//-----------------------------------------------------------------------
// <copyright file="UriStringToImageConverter.cs" company="Development In Progress Ltd">
//     Copyright © 2012. All rights reserved.
// </copyright>
// <author>Grant Colley</author>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DevelopmentInProgress.Wpf.Host.Converters
{
    /// <summary>
    /// Converts a Uri string to an image.
    /// </summary>
    public sealed class UriStringToImageConverter : IValueConverter
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
            try
            {
                return new BitmapImage(new Uri((string)value, UriKind.RelativeOrAbsolute));
            }
            catch
            {
                return new BitmapImage(
                    new Uri(@"/DevelopmentInProgress.Wpf.Host;component/Images/GroupListItem.png",
                    UriKind.RelativeOrAbsolute));
            }
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