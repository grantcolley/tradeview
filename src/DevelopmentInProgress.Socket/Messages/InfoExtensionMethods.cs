using DevelopmentInProgress.Socket.Server;
using System.Linq;

namespace DevelopmentInProgress.Socket.Messages
{
    public static class InfoExtensionMethods
    {
        public static ConnectionInfo GetConnectionInfo(this Connection connection)
        {
            return new ConnectionInfo { Name = connection.Name, ConnectionId = connection.ConnectionId };
        }

        public static ChannelInfo GetChannelInfo(this Channel channel)
        {
            var connectionInfos = (from connection in channel.Connections select connection.Value.GetConnectionInfo()).ToList();
            var channelInfo = new ChannelInfo { Name = channel.Name, ConnectionId = channel.Name };
            channelInfo.Connections.AddRange(connectionInfos);
            return channelInfo;
        }
    }
}