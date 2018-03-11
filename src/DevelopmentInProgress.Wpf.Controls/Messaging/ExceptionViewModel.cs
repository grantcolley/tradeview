//-----------------------------------------------------------------------
// <copyright file="ExceptionViewModel.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2013. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DevelopmentInProgress.Wpf.Controls.Messaging
{
    /// <summary>
    /// The view model for the <see cref="ExceptionView"/> class.
    /// </summary>
    public class ExceptionViewModel
    {
        /// <summary>
        /// Initializes a new insatance of the <see cref="ExceptionViewModel"/> class.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="stackTrace">The stacktrace to display.</param>
        public ExceptionViewModel(string message, string stackTrace)
        {
            Message = message;
            StackTrace = stackTrace;
        }

        /// <summary>
        /// Gets the type of image to display for the error once 
        /// converted to image by <see cref="MessageTextToImageConverter"/>.
        /// </summary>
        public string Type { get { return "Error"; } }

        /// <summary>
        /// Gets the type of image to display for the clipboard
        /// once converted to image by <see cref="MessageTextToImageConverter"/>.
        /// </summary>
        public string Clipboard { get { return "Clipboard"; } }

        /// <summary>
        /// Gets the message to display.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the stacktrace to display.
        /// </summary>
        public string StackTrace { get; private set; }
    }
}
