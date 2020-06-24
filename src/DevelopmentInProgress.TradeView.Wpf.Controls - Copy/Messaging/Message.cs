//-----------------------------------------------------------------------
// <copyright file="Message.cs" company="Development In Progress Ltd">
//     Copyright © Development In Progress Ltd 2013. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace DevelopmentInProgress.TradeView.Wpf.Controls.Messaging
{
    /// <summary>
    /// Details of the message to be displayed.
    /// </summary>
    public class Message : INotifyPropertyChanged
    {
        private bool isVisible;

        /// <summary>
        /// Initializes a new instance of the Message.
        /// </summary>
        public Message()
        {
            IsVisible = true;
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Notification event raised when a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the type of message to display.
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// Gets or sets the message text to display.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the verbose text to display.
        /// </summary>
        public string TextVerbose { get; set; }

        /// <summary>
        /// Gets or sets the title of the message to display.
        /// </summary>
        public string Title { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets a text representation of the message to be 
        /// converted to image by <see cref="MessageTextToImageConverter"/>.
        /// </summary>
        public string Type
        {
            get
            {
                return MessageType == MessageType.Error ? "Error"
                    : MessageType == MessageType.Warn ? "Warn"
                    : MessageType == MessageType.Question ? "Question" : "Info";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Message"/> is visible or not.
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler != null)
            {
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
