using DevelopmentInProgress.Socket.Server;
using System.Collections.Generic;

namespace DevelopmentInProgress.Socket.Messages
{
    /// <summary>
    /// Information about an active <see cref="Channel"/> managed by the <see cref="SocketServer"/>.
    /// </summary>
    public class ChannelInfo : IInfo
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ChannelInfo"/>.
        /// </summary>
        public ChannelInfo()
        {
            Connections = new List<ConnectionInfo>();
        }

        /// <summary>
        /// Gets or sets the name of the channel.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the connection id;
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// Gets or sets a list containing information about the  
        /// connection's that have subscribed to the channel.
        /// </summary>
        public List<ConnectionInfo> Connections { get; set; }
    }
}
