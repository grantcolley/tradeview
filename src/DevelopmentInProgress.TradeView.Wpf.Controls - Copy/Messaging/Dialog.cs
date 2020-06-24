//-----------------------------------------------------------------------
// <copyright file="Messaging.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2013. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.Messaging
{
    /// <summary>
    /// Shows a message and returns the users response.
    /// </summary>
    public static class Dialog
    {
        /// <summary>
        /// Shows a message and returns the users response.
        /// </summary>
        /// <param name="text">The message text.</param>
        /// <param name="title">The message title.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="buttons">The buttons to display.</param>
        /// <param name="copyToClipboardEnabled">Enable copying message to the clipboard. Defaults is false.</param>
        /// <returns>The users response to the message.</returns>
        public static MessageBoxResult ShowMessage(string text, string title, MessageType messageType,
            MessageBoxButtons buttons, bool copyToClipboardEnabled = false)
        {
            var message = new MessageBoxSettings()
            {
                Text = text,
                Title = title,
                MessageType = messageType,
                MessageBoxButtons = buttons,
                CopyToClipboardEnabled = copyToClipboardEnabled
            };

            return ShowMessage(message);
        }

        /// <summary>
        /// Shows a message and returns the users response.
        /// </summary>
        /// <param name="messageBoxSettings">The message to show including text, title and 
        /// message type, buttons and whether to enable copying the message to a clipboard</param>
        /// <returns>The users response to the message.</returns>
        public static MessageBoxResult ShowMessage(MessageBoxSettings messageBoxSettings)
        {
            var model = new MessageBoxViewModel(messageBoxSettings);
            var view = new MessageBoxView();
            view.DataContext = model;
            view.ShowDialog();
            messageBoxSettings.MessageBoxResult = model.MessageBoxResult;
            return messageBoxSettings.MessageBoxResult;
        }

        /// <summary>
        /// Shows an exception message with stacktrace.
        /// </summary>
        /// <param name="exception">The exception to show.</param>
        public static void ShowException(Exception exception)
        {
            ShowException(exception.Message, exception.StackTrace);
        }

        /// <summary>
        /// Shows an exception message with stacktrace.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="stackTrace">The stacktrace.</param>
        public static void ShowException(string exceptionMessage, string stackTrace)
        {
            var errorView = new ExceptionView();
            var errorViewModel = new ExceptionViewModel(exceptionMessage, stackTrace);
            errorView.DataContext = errorViewModel;
            errorView.ShowDialog();
        }
    }
}
