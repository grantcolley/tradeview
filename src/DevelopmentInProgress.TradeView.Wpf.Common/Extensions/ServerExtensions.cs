using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class ServerExtensions
    {
        public static Server ToWpfServer(this Core.Server.Server server)
        {
            return new Server
            {
                Name = server.Name,
                Url = server.Url,
                MaxDegreeOfParallelism = server.MaxDegreeOfParallelism,
                Enabled = server.Enabled
            };
        }

        public static Core.Server.Server ToCoreServer(this Server server)
        {
            return new Core.Server.Server
            {
                Name = server.Name,
                Url = server.Url,
                MaxDegreeOfParallelism = server.MaxDegreeOfParallelism,
                Enabled = server.Enabled
            };
        }
    }
}