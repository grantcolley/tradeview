using DevelopmentInProgress.Socket.Server;
using System.Collections.Generic;

namespace DevelopmentInProgress.Socket.Messages
{
    /// <summary>
    /// Contains information about active <see cref="Connection"/>'s 
    /// and <see cref="Channel"/>'s managed by the <see cref="SocketServer"/>.
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ServerInfo"/> class.
        /// </summary>
        public ServerInfo()
        {
            Connections = new List<ConnectionInfo>();
            Channels = new List<ChannelInfo>();
        }

        /// <summary>
        /// Gets or sets list of active <see cref="ConnectionInfo"/>'s managed by the <see cref="SocketServer"/>.
        /// </summary>
        public List<ConnectionInfo> Connections { get; private set; }

        /// <summary>
        /// Gets or sets list of active <see cref="ChannelInfo"/>'s managed by the <see cref="SocketServer"/>.
        /// </summary>
        public List<ChannelInfo> Channels { get; private set; }
    }
}
