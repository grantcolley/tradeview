using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class ServerExtensions
    {
        public static Server ToWpfServer(this Interface.Server.Server server)
        {
            return new Server
            {
                Name = server.Name,
                Url = server.Url
            };
        }

        public static Interface.Server.Server ToInterfaceServer(this Server server)
        {
            return new Interface.Server.Server
            {
                Name = server.Name,
                Url = server.Url
            };
        }
    }
}