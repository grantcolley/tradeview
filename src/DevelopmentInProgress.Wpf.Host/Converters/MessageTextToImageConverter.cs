//-----------------------------------------------------------------------
// <copyright file="MessageTextToImageConverter.cs" company="Development In Progress Ltd">
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
    /// Converts the message type to the image to display for the message.
    /// </summary>
    public sealed class MessageTextToImageConverter : IValueConverter
    {
        /// <summary>
        /// Converts the value to the converted type.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture information.</param>
        /// <returns>A converted type.</returns>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            try
            {
                String size = String.Empty;
                if (parameter != null
                    && parameter.ToString().ToUpper().Equals("MSGBOX"))
                {
                    size = "32";
                }

                string image = value.ToString();
                switch (image.ToLower())
                {
                    case "info":
                        return new BitmapImage(new Uri(String.Format(@"..\Images\Information{0}.png", size), UriKind.RelativeOrAbsolute));
                    case "warn":
                        return new BitmapImage(new Uri(String.Format(@"..\Images\Warning{0}.png", size), UriKind.RelativeOrAbsolute));
                    case "error":
                        return new BitmapImage(new Uri(String.Format(@"..\Images\Error{0}.png", size), UriKind.RelativeOrAbsolute));
                    case "question":
                        return new BitmapImage(new Uri(String.Format(@"..\Images\Question{0}.png", size), UriKind.RelativeOrAbsolute));
                    case "clipboard":
                        return new BitmapImage(new Uri(String.Format(@"..\Images\Clipboard{0}.png", size), UriKind.RelativeOrAbsolute));
                    default:
                        return null;
                }
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