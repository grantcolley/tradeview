using DevelopmentInProgress.TradeView.Core.Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Data
{
    public interface ITradeViewConfigurationServer
    {
        Task<ServerConfiguration> GetServerConfiguration();
        Task<List<Server>> GetServersAsync();
        Task<Server> GetServerAsync(string serverName);
        Task SaveServerAsync(Server server);
        Task DeleteServerAsync(Server server);
    }
}