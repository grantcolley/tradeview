//-----------------------------------------------------------------------
// <copyright file="MessageBoxView.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2013. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.Messaging
{
    /// <summary>
    /// Interaction logic for MessageBoxView.xaml for displaying a message.
    /// </summary>
    partial class MessageBoxView : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxView"/> class.
        /// </summary>
        public MessageBoxView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles a button click on the view and raises the 
        /// OnButtonClicked event handler on the <see cref="MessageBoxViewModel"/>.
        /// </summary>
        /// <param name="sender">The button clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var messageBoxViewModel = (MessageBoxViewModel)this.DataContext;
            messageBoxViewModel.OnButtonClick(button.Content.ToString());
            this.Close();
        }

        /// <summary>
        /// Copies the message and stack trace to the clipboard.
        /// </summary>
        /// <param name="sender">The button clicked.</param>
        /// <param name="e">Event arguments.</param>
        public void CopyClick(object sender, RoutedEventArgs e)
        {
            var messageBoxViewModel = (MessageBoxViewModel)this.DataContext;
            messageBoxViewModel.OnCopyClick();
        }

        /// <summary>
        /// Handles windows closing event on the view and raises the 
        /// OnButtonClicked event handler for a cancel action 
        /// on the <see cref="MessageBoxViewModel"/>.
        /// </summary>
        /// <param name="sender">The button clicked.</param>
        /// <param name="e">Event arguments.</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var messageBoxViewModel = (MessageBoxViewModel)this.DataContext;
            messageBoxViewModel.OnButtonClick("Cancel");
        }
    }
}
