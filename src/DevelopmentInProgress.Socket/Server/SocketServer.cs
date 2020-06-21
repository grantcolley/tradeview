using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.Socket.Messages;
using System.Linq;
using System.Collections.Generic;

namespace DevelopmentInProgress.Socket.Server
{
    /// <summary>
    /// The abstract <see cref="SocketServer"/> base class.
    /// </summary>
    public abstract class SocketServer
    {
        private ConnectionManager connectionManager;
        private ChannelManager channelManager;

        /// <summary>
        /// Creates a new instance of the <see cref="SocketServer"/> base class.
        /// </summary>
        protected SocketServer()
        {
            connectionManager = new ConnectionManager();
            channelManager = new ChannelManager();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SocketServer"/>.
        /// Use when injecting singleton <see cref="ConnectionManager"/> 
        /// and <see cref="ChannelManager"/> is preferred.
        /// </summary>
        /// <param name="connectionManager">An instance of the <see cref="ConnectionManager"/>.</param>
        /// <param name="channelManager">An instance of the <see cref="ChannelManager"/>.</param>
        protected SocketServer(ConnectionManager connectionManager, ChannelManager channelManager)
        {
            this.connectionManager = connectionManager;
            this.channelManager = channelManager;
        }

        /// <summary>
        /// Receive a message from a <see cref="WebSocket"/>.
        /// </summary>
        /// <param name="webSocket">The <see cref="WebSocket"/>.</param>
        /// <param name="message">The <see cref="Message"/>.</param>
        /// <returns>A <see="Task"/>.</returns>
        public abstract Task ReceiveAsync(WebSocket webSocket, Message message);

        /// <summary>
        /// Handle a <see cref="WebSocket"/> client connection.
        /// </summary>
        /// <param name="websocket">The <see cref="WebSocket"/>.</param>
        /// <param name="clientId">The identifier of the <see cref="WebSocket"/>.</param>
        /// <param name="data">The data accompanying by the <see cref="WebSocket"/>.</param>
        /// <returns>A <see="Task"/>.</returns>
        public abstract Task OnClientConnectAsync(WebSocket websocket, string clientId, string data);

        /// <summary>
        /// Removes the <see cref="WebSocket"/> <see cref="ConnectionManager"/>'s web sockets dictionary, 
        /// calls the web sockets CloseAsync method and then disposes it.
        /// </summary>
        /// <param name="webSocket">The <see cref="WebSocket"/> to remove.</param>
        /// <returns>The <see cref="Connection"/> for the <see cref="WebSocket"/>.</returns>
        public virtual async Task<Connection> OnClientDisonnectAsync(WebSocket webSocket)
        {
            if (webSocket == null)
            {
                throw new ArgumentNullException(nameof(webSocket));
            }

            if (connectionManager.TryRemoveWebSocketConnection(webSocket, out Connection connection))
            {
                connection.Channels.All(c => c.Value.Connections.TryRemove(connection.ConnectionId, out Connection removedConnection));

                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, 
                    $"WebSocket {connection.ConnectionId} closed by {typeof(SocketServer).Name}", 
                    CancellationToken.None).ConfigureAwait(false);

                webSocket.Dispose();
                
                return connection;
            }

            return null;
        }

        /// <summary>
        /// Adds the <see cref="WebSocket"/> to the <see cref="ConnectionManager"/>'s web sockets dictionary.
        /// </summary>
        /// <param name="websocket">The <see cref="WebSocket"/> to add.</param>
        /// <returns>The <see cref="Connection"/> for the <see cref="WebSocket"/>.</returns>
        public virtual Task<Connection> AddWebSocketAsync(WebSocket websocket)
        {
            if (connectionManager.TryAddWebSocketConnection(websocket, out Connection connection))
            {
                return Task.FromResult<Connection>(connection);
            }

            return null;
        }

        /// <summary>
        /// Send a message to a <see cref="WebSocket"/> client.
        /// </summary>
        /// <param name="connectionId">The connection Id of the <see cref="WebSocket"/> client.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task SendMessageAsync(string connectionId, Message message)
        {
            var connection = connectionManager.GetConnection(connectionId);
            if (connection != null)
            {
                await SendMessageAsync(connection.WebSocket, message).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Send messages to a list of connections where the message for each connection is returned by a delegate.
        /// </summary>
        /// <param name="connections">The connections to send messages to.</param>
        /// <param name="getMessage">The delegate to get the message for each connection.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task SendMessageAsync(List<Connection> connections, Func<Connection, Message> getMessage)
        {
            var webSockets = (from connection in connections select SendMessageAsync(connection.WebSocket, getMessage(connection))).ToList();

            await Task.WhenAll(webSockets.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message to all <see cref="WebSocket"/> clients.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task SendMessageToAllAsync(Message message)
        {
            var json = JsonConvert.SerializeObject(message);

            var connections = connectionManager.GetConnections();

            var webSockets = from connection in connections select SendMessageAsync(connection.WebSocket, json);

            await Task.WhenAll(webSockets.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message to all <see cref="WebSocket"/> clients.
        /// </summary>
        /// <param name="channelName">The channel name.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task SendMessageToChannelAsync(string channelName, Message message)
        {
            var channel = channelManager.GetChannel(channelName);

            if(channel == null)
            {
                return;
            }

            await SendMessageToChannelAsync(channel, message).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message to all <see cref="WebSocket"/> clients.
        /// </summary>
        /// <param name="channelName">The channel name.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task SendMessageToChannelAsync(Channel channel, Message message)
        {
            if (channel == null)
            {
                return;
            }

            var json = JsonConvert.SerializeObject(message);

            var webSockets = from connection in channel.Connections.Values.ToArray() select SendMessageAsync(connection.WebSocket, json);

            await Task.WhenAll(webSockets.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message to a <see cref="WebSocket"/> client.
        /// </summary>
        /// <param name="webSocket">The <see cref="WebSocket"/> to send the message to.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task SendMessageAsync(WebSocket webSocket, Message message)
        {
            var json = JsonConvert.SerializeObject(message);

            await SendMessageAsync(webSocket, json).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a message to a <see cref="WebSocket"/> client.
        /// </summary>
        /// <param name="webSocket">The <see cref="WebSocket"/> to send the message to.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task SendMessageAsync(WebSocket webSocket, string message)
        {
            if(webSocket == null)
            {
                throw new ArgumentNullException(nameof(webSocket));
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!webSocket.State.Equals(WebSocketState.Open))
            {
                return;
            }

            await webSocket.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length),
                WebSocketMessageType.Text, true, CancellationToken.None)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribe to a <see cref="Channel"/>. If the <see cref="Channel"/> 
        /// doesn't exist then create one.
        /// </summary>
        /// <param name="channelName">The channel to subscribe to.</param>
        /// <param name="webSocket">The connection subscribing to the channel.</param>
        /// <returns>The <see cref="Channel"/>.</returns>
        public Channel SubscribeToChannel(string channelName, WebSocket webSocket)
        {
            var connection = connectionManager.GetConnection(webSocket);
            return channelManager.SubscribeToChannel(channelName, connection);
        }

        /// <summary>
        /// Unsubscribe from a <see cref="Channel"/>. If the 
        /// <see cref="Channel"/> no longer has any <see cref="Connection"/>'s 
        /// then remove the <see cref="Channel"/>.
        /// </summary>
        /// <param name="channelName">The channel to unsubscribe from.</param>
        /// <param name="webSocket">The connection unsubscribing to the channel.</param>
        /// <returns>The <see cref="Channel"/>.</returns>
        public Channel UnsubscribeFromChannel(string channelName, WebSocket webSocket)
        {
            var connection = connectionManager.GetConnection(webSocket);
            return channelManager.UnsubscribeFromChannel(channelName, connection);
        }

        /// <summary>
        /// Get a <see cref="Channel"/>.
        /// </summary>
        /// <param name="channelName">The <see cref="Channel"/> to get.</param>
        /// <returns>The <see cref="Channel"/>.</returns>
        public Channel GetChannel(string channelName)
        {
            return channelManager.GetChannel(channelName);
        }

        /// <summary>
        /// Remove a <see cref="Channel"/>.
        /// </summary>
        /// <param name="channelName">The <see cref="Channel"/> to remove.</param>
        /// <param name="channel">The removed channel.</param>
        /// <returns>True if successful, else false.</returns>
        public bool TryRemoveChannel(string channelName, out Channel channel)
        {
            return channelManager.TryRemoveChannel(channelName, out channel);
        }

        /// <summary>
        /// Get information about active <see cref="SocketServer"/> connections and channels.
        /// </summary>
        /// <returns></returns>
        public ServerInfo GetServerInfo()
        {
            return new ServerInfo
            {
                Channels = GetChannelInfos(),
                Connections = GetConnectionInfos()
            };
        }

        /// <summary>
        /// Gets a <see cref="Connection"/>.
        /// </summary>
        /// <param name="connectionId">The connection id.</param>
        /// <returns>The <see cref="Connection"/>.</returns>
        public Connection GetConnection(string connectionId)
        {
            return connectionManager.GetConnection(connectionId);
        }

        private List<ChannelInfo> GetChannelInfos()
        {
            var channelInfos = channelManager.GetChannelInfos();
            return channelInfos;
        }

        private List<ConnectionInfo> GetConnectionInfos()
        {
            var connectionInfos = connectionManager.GetConnectionInfos();
            return connectionInfos;
        }
    }
}