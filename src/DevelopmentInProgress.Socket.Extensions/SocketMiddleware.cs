using DevelopmentInProgress.Socket.Messages;
using DevelopmentInProgress.Socket.Server;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Socket.Extensions
{
    /// <summary>
    /// The middleware for handling requests to and responses from <see cref="SocketServer"/>.
    /// </summary>
    public class SocketMiddleware
    {
        private readonly SocketServer socketServer;


        /// <summary>
        /// Creates an instance of the <see cref="SocketMiddleware"/> class.
        /// </summary>
        /// <param name="next">The <see cref="RequestDelegate"/>.</param>
        /// <param name="socketServer">A specialised instance of a class inheriting <see cref="SocketServer"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        public SocketMiddleware(RequestDelegate next, SocketServer socketServer)
        {
            this.socketServer = socketServer;
        }

        /// <summary>
        /// Receives a request to the class inheriting <see cref="SocketServer"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <returns>The response.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            try
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);

                    var clientId = context.Request.Query["clientId"];
                    var data = context.Request.Query["data"];

                    await socketServer.OnClientConnectAsync(webSocket, clientId, data).ConfigureAwait(false);

                    await Receive(webSocket).ConfigureAwait(false);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (WebSocketException wsex) when (wsex.WebSocketErrorCode.Equals(WebSocketError.ConnectionClosedPrematurely))
            {
                // The remote party closed the WebSocket connection
                // without completing the close handshake.
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.Clear();
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.WriteAsync(JsonConvert.SerializeObject(ex)).ConfigureAwait(false);
            }
        }

        private async Task Receive(WebSocket webSocket)
        {
            try
            {
                var buffer = new byte[1024 * 4];
                var messageBuilder = new StringBuilder();

                while (webSocket.State.Equals(WebSocketState.Open))
                {
                    WebSocketReceiveResult webSocketReceiveResult;

                    messageBuilder.Clear();

                    do
                    {
                        webSocketReceiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);

                        if (webSocketReceiveResult.MessageType.Equals(WebSocketMessageType.Close))
                        {
                            await socketServer.OnClientDisonnectAsync(webSocket).ConfigureAwait(false);
                            continue;
                        }

                        if (webSocketReceiveResult.MessageType.Equals(WebSocketMessageType.Text))
                        {
                            messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, webSocketReceiveResult.Count));
                            continue;
                        }
                    }
                    while (!webSocketReceiveResult.EndOfMessage);

                    if (messageBuilder.Length > 0)
                    {
                        var json = messageBuilder.ToString();

                        var message = JsonConvert.DeserializeObject<Message>(json);

                        await socketServer.ReceiveAsync(webSocket, message).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                webSocket?.Dispose();
            }
        }
    }
}
