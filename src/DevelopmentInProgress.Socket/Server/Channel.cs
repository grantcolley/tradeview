using System.Collections.Concurrent;

namespace DevelopmentInProgress.Socket.Server
{
    /// <summary>
    /// A <see cref="Channel"/> is a group of <see cref="Connection"/>'s 
    /// where a messgae from one of the connections is broadcast 
    /// to all connections subscribed to the channel.
    /// </summary>
    public sealed class Channel
    {
        /// <summary>
        /// Creates an instance of the <see cref="Channel"/> class.
        /// </summary>
        internal Channel()
        {
            Connections = new ConcurrentDictionary<string, Connection>();
        }

        /// <summary>
        /// Gets or sets the channel name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets a dictionary of <see cref="Connection"/>'s.
        /// </summary>
        public ConcurrentDictionary<string, Connection> Connections { get; set; }
    }
}
