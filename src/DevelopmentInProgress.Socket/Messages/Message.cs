using System;
using DevelopmentInProgress.Socket.Client;
using DevelopmentInProgress.Socket.Server;

namespace DevelopmentInProgress.Socket.Messages
{
    /// <summary>
    /// The message class.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Creates an instance of the base message.
        /// </summary>
        public Message()
        {
            SentOn = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the date / time the message is sent.
        /// </summary>
        public DateTime SentOn { get; set; }

        /// <summary>
        /// Gets or sets the senders connection id.
        /// </summary>
        public string SenderConnectionId { get; set; }

        /// <summary>
        /// Gets or sets the recipients connection id of the message which is either another <see cref="Connection"/> or a <see cref="Channel"/>.
        /// </summary>
        public string RecipientConnectionId { get; set; }

        /// <summary>
        /// Gets or sets the type of message sent from the <see cref="DipSocketClient"/> to the <see cref="DipSocketServer"/>.
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// Gets or sets the serialised message data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// The name of the method to invoke on the client.
        /// </summary>
        public string MethodName { get; set; }
    }
}
