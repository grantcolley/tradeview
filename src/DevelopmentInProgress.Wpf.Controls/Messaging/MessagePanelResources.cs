//-----------------------------------------------------------------------
// <copyright file="MessagePanelResources.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2017. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls;
using System.Windows.Input;

namespace DevelopmentInProgress.Wpf.Controls.Messaging
{
    /// <summary>
    /// Partial class providing code-behind supporting for events raised in MessagePanelResources.xaml.
    /// </summary>
    public partial class MessagePanelResources
    {
        private void MessageMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var contentControl = sender as ContentControl;
            if (contentControl == null)
            {
                return;
            }

            var message = contentControl.DataContext as Message;
            if (message == null)
            {
                return;
            }

            var messagePanel = contentControl.Tag as MessagePanel;
            if (messagePanel == null)
            {
                return;
            }

            var messageBoxSettings = new MessageBoxSettings
            {
                Text = string.IsNullOrWhiteSpace(message.TextVerbose) ? message.Text : message.TextVerbose,
                Title = message.Title,
                MessageType = message.MessageType,
                MessageBoxButtons = MessageBoxButtons.Ok,
                CopyToClipboardEnabled = true,
                MessageBoxText = new MessageBoxText { TextAlignment = messagePanel.ShowMessageTextAlignment, MaxWidth = messagePanel.ShowMessageTextAreaMaxWidth }
            };

            Dialog.ShowMessage(messageBoxSettings);
        }
    }
}
