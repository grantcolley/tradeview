using DevelopmentInProgress.TradeView.Interface.Server;
using System;

namespace DevelopmentInProgress.TradeView.Interface.Extensions
{
    public static class ServerExtensions
    {
        public static ServerNotification GetNotification(this Server.Server server)
        {
            return new ServerNotification
            {
                 Machine = Environment.MachineName
            };
        }
    }
}
