//-----------------------------------------------------------------------
// <copyright file="MessagePanelResources.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2017. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Controls;
using System.Windows.Input;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.Messaging
{
    /// <summary>
    /// Partial class providing code-behind supporting for events raised in MessagePanelResources.xaml.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1010:Generic interface should also be implemented")]
    public partial class MessagePanelResources
    {
        private void MessageMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!(sender is ContentControl contentControl))
            {
                return;
            }

            if (!(contentControl.DataContext is Message message))
            {
                return;
            }

            if (!(contentControl.Tag is MessagePanel messagePanel))
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
                MessageBoxText = new MessageBoxText { TextAlignment = messagePanel.ShowMessageTextAlignment }
            };

            Dialog.ShowMessage(messageBoxSettings);
        }
    }
}
