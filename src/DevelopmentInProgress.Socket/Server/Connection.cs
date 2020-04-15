using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace DevelopmentInProgress.Socket.Server
{
    /// <summary>
    /// Represents a <see cref="WebSocket"/> connection.
    /// </summary>
    public sealed class Connection
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="webSocket">The <see cref="WebSocket"/> the connection represents.</param>
        internal Connection(WebSocket webSocket)
        {
            WebSocket = webSocket;
            Channels = new ConcurrentDictionary<string, Channel>();
        }
        
        /// <summary>
        /// Gets or sets the connection name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the connection id.
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// Gets the connections <see cref="WebSocket"/>.
        /// </summary>
        public WebSocket WebSocket { get; }

        /// <summary>
        /// Gets or sets a dictionary of <see cref="Channel"/>'s the connection has subscribed to.
        /// </summary>
        public ConcurrentDictionary<string, Channel> Channels { get; set; }
    }
}