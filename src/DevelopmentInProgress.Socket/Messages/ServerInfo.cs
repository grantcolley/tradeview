using System.Collections.Generic;
using DevelopmentInProgress.Socket.Server;

namespace DevelopmentInProgress.Socket.Messages
{
    /// <summary>
    /// Contains information about active <see cref="Connection"/>'s 
    /// and <see cref="Channel"/>'s managed by the <see cref="DipSocketServer"/>.
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// Gets or sets list of active <see cref="ConnectionInfo"/>'s managed by the <see cref="DipSocketServer"/>.
        /// </summary>
        public List<ConnectionInfo> Connections { get; set; }

        /// <summary>
        /// Gets or sets list of active <see cref="ChannelInfo"/>'s managed by the <see cref="DipSocketServer"/>.
        /// </summary>
        public List<ChannelInfo> Channels { get; set; }
    }
}
