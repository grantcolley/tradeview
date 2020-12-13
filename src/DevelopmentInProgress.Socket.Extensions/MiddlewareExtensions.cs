using DevelopmentInProgress.Socket.Server;
using Microsoft.AspNetCore.Builder;
using System;

namespace DevelopmentInProgress.Socket.Extensions
{
    /// <summary>
    /// Static class containing the extension method for adding the <see cref="SocketMiddleware"/> to <see cref="IApplicationBuilder"/>. 
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Extension method for adding the <see cref="SocketMiddleware"/> to <see cref="IApplicationBuilder"/>. 
        /// </summary>
        /// <typeparam name="T">A type that inherits <see cref="SocketServer"/>.</typeparam>
        /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="route">The route to map to the <see cref="SocketMiddleware"/>.</param>
        /// <returns>An instance of <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseSocket<T>(this IApplicationBuilder builder, string route) where T : SocketServer
        {
            if(builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.UseWebSockets();
            var webSocketServer = (T)builder.ApplicationServices.GetService(typeof(T));
            return builder.Map(route, (applicationBuilder) => applicationBuilder.UseMiddleware<SocketMiddleware>(webSocketServer));
        }
    }
}
