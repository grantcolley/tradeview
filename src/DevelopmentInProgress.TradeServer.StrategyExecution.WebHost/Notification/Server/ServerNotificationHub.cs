using DevelopmentInProgress.Socket.Messages;
using DevelopmentInProgress.Socket.Server;
using DevelopmentInProgress.TradeView.Core.Server;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Server
{
    public class ServerNotificationHub : SocketServer, IServerNotification
    {
        private readonly IServerMonitor serverMonitor;

        public ServerNotificationHub(ConnectionManager connectionManager, ChannelManager channelManager, IServerMonitor serverMonitor)
            : base(connectionManager, channelManager)
        {
            this.serverMonitor = serverMonitor;
        }

        public event EventHandler<ServerNotificationEventArgs> ServerNotification;

        public async override Task OnClientConnectAsync(WebSocket websocket, string clientId, string data)
        {
            if (string.IsNullOrWhiteSpace(serverMonitor.Name))
            {
                throw new Exception($"The server {nameof(serverMonitor.Name)} has not been set.");
            }

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!serverMonitor.Name.Equals(data, StringComparison.Ordinal))
            {
                throw new ArgumentNullException($"The server name {data} in the client request does not match the server channel {serverMonitor.Name}");
            }

            var connection = await base.AddWebSocketAsync(websocket).ConfigureAwait(false);

            connection.Name = clientId;

            SubscribeToChannel(serverMonitor.Name, websocket);

            var connectionInfo = connection.GetConnectionInfo();

            var json = JsonConvert.SerializeObject(connectionInfo);

            var message = new Message { MethodName = "OnConnected", SenderConnectionId = "Server", Data = json };

            await SendMessageAsync(websocket, message).ConfigureAwait(false);

            OnServerNotification();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Exception fed back to subscriber.")]
        public async override Task ReceiveAsync(WebSocket webSocket, Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            try
            {
                switch (message.MessageType)
                {
                    case MessageType.UnsubscribeFromChannel:
                        UnsubscribeFromChannel(serverMonitor.Name, webSocket);
                        break;
                }

                OnServerNotification();
            }
            catch (Exception ex)
            {
                var errorMessage = new Message { MethodName = message.MethodName, SenderConnectionId = message.SenderConnectionId, Data = $"{MessageType.UnsubscribeFromChannel} Error : {ex.Message}" };
                await SendMessageAsync(webSocket, errorMessage).ConfigureAwait(false);
            }
        }

        private void OnServerNotification()
        {
            var serverNotification = ServerNotification;
            serverNotification?.Invoke(this, new ServerNotificationEventArgs());
        }
    }
}
