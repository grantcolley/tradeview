using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public interface IServerService
    {
        Task<List<Server>> GetServers();
        Task<Server> GetServer(string serverName);
        Task SaveServer(Server server);
        Task DeleteServer(Server server);
    }
}