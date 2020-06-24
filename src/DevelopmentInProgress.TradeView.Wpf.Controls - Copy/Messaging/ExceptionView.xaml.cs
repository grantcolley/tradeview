using System;
using System.Windows;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.Messaging
{
    /// <summary>
    /// Interaction logic for ErrorView.xaml which displays an error message with stacktrace.
    /// </summary>
    public partial class ExceptionView : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionView"/> class.
        /// </summary>
        public ExceptionView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the button clicked event to close the window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Copies the message and stack trace to the clipboard.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void CopyClick(object sender, RoutedEventArgs e)
        {
            string text = String.Format("Error: {0}\r\n\r\nStackTrace:\r\n {1}", txtMessage.Text, txtStackTrace.Text);
            Clipboard.Clear();
            Clipboard.SetText(text);
        }
    }
}
