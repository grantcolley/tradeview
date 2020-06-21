using DevelopmentInProgress.Socket.Messages;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DevelopmentInProgress.Socket.Tests")]
namespace DevelopmentInProgress.Socket.Server
{
    /// <summary>
    /// The <see cref="ChannelManager"/> class stores and manages access to a 
    /// <see cref="ConcurrentDictionary{string, Channel}"/> of <see cref="Channel"/>'s 
    /// for client <see cref="WebSocket"/>'s to subscribe to.
    /// </summary>
    public sealed class ChannelManager
    {
        private readonly ConcurrentDictionary<string, Channel> channels;

        /// <summary>
        /// Creates a new instance of the <see cref="ChannelManager"/> class.
        /// </summary>
        public ChannelManager()
        {
            channels = new ConcurrentDictionary<string, Channel>();
        }

        internal List<Channel> GetChannels()
        {
            return channels.Values.ToList();
        }

        internal List<ChannelInfo> GetChannelInfos()
        {
            return channels.Values.Select(c => c.GetChannelInfo()).ToList();
        }

        internal Channel GetChannel(string channelName)
        {
            if (channels.TryGetValue(channelName, out Channel channel))
            {
                return channel;
            }

            return null;
        }

        internal Channel SubscribeToChannel(string channelName, Connection connection)
        {
            var channel = channels.GetOrAdd(channelName, name =>
            {
                return new Channel { Name = name};
            });

            if (channel.Connections.TryAdd(connection.ConnectionId, connection))
            {
                connection.Channels.TryAdd(channel.Name, channel);
                return channel;
            }

            return null;
        }

        internal Channel UnsubscribeFromChannel(string channelName, Connection connection)
        {
            if (!channels.ContainsKey(channelName))
            {
                return null;
            }

            if (channels.TryGetValue(channelName, out Channel channel))
            {
                channel.Connections.TryRemove(connection.ConnectionId, out Connection removedConnection);

                removedConnection.Channels.TryRemove(channel.Name, out Channel removedConnectionChannel);

                if (channel.Connections.Any())
                {
                    return channel;
                }

                if (TryRemoveChannel(channelName, out Channel removedChannel))
                {
                    return removedChannel;
                }
            }

            return null;
        }

        internal bool TryRemoveChannel(string channelName, out Channel channel)
        {
            if (channels.TryRemove(channelName, out channel))
            {
                var connections = channel.Connections.Keys;
                foreach(var connectionId in connections)
                {
                    if(channel.Connections.TryRemove(connectionId, out Connection connection))
                    {
                        connection.Channels.TryRemove(channelName, out Channel removed);
                    }
                }

                return true;
            }

            return false;
        }
    }
}