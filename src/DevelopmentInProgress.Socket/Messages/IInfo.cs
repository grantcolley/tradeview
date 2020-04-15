using DevelopmentInProgress.Socket.Server;

namespace DevelopmentInProgress.Socket.Messages
{
    /// <summary>
    /// Information about a active <see cref="Connection"/> or <see cref="Channel"/>.
    /// </summary>
    public interface IInfo
    {
        /// <summary>
        /// Gets or sets the name of the channel.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the connection id;
        /// </summary>
        string ConnectionId { get; set; }
    }
}
