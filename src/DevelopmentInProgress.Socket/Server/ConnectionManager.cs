using DevelopmentInProgress.Socket.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DevelopmentInProgress.Socket.Tests")]
namespace DevelopmentInProgress.Socket.Server
{
    /// <summary>
    /// The <see cref="ConnectionManager"/> class stores and manages access to a 
    /// <see cref="ConcurrentDictionary{string, Connection}"/> of client <see cref="WebSocket"/>'s.
    /// </summary>
    public sealed class ConnectionManager
    {
        private readonly ConcurrentDictionary<string, Connection> connections;

        /// <summary>
        /// Creates a new instance of the <see cref="ConnectionManager"/> class.
        /// </summary>
        public ConnectionManager()
        {
            connections = new ConcurrentDictionary<string, Connection>();
        }

        internal List<Connection> GetConnections()
        {
            return connections.Values.ToList();
        }

        internal List<ConnectionInfo> GetConnectionInfos()
        {
            return connections.Values.Select(c => c.GetConnectionInfo()).ToList();
        }

        internal Connection GetConnection(string connectionId)
        {
            if (connections.TryGetValue(connectionId, out Connection connection))
            {
                return connection;
            }

            return null;
        }

        internal Connection GetConnection(WebSocket webSocket)
        {
            foreach (var kvp in connections)
            {
                if (kvp.Value.WebSocket == webSocket)
                {
                    return kvp.Value;
                }
            }

            return null;
        }

        internal string GetConnectionId(WebSocket webSocket)
        {
            foreach(var kvp in connections)
            {
                if(kvp.Value.WebSocket == webSocket)
                {
                    return kvp.Key;
                }
            }

            return null;
        }

        internal bool TryAddWebSocketConnection(WebSocket webSocket, out Connection connection)
        {
            var connectionId = Guid.NewGuid().ToString();
            connection = new Connection(webSocket) { ConnectionId = connectionId };
            return connections.TryAdd(connectionId, connection);
        }

        internal bool TryRemoveWebSocketConnection(WebSocket webSocket, out Connection connection)
        {
            var connectionId = GetConnectionId(webSocket);
            return connections.TryRemove(connectionId, out connection);
        }
    }
}