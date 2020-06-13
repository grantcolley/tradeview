using DevelopmentInProgress.TradeView.Core.Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Data
{
    public interface ITradeViewConfigurationServer
    {
        Task<ServerConfiguration> GetServerConfiguration();
        Task<List<TradeServer>> GetTradeServersAsync();
        Task<TradeServer> GetTradeServerAsync(string tradeServerName);
        Task SaveTradeServerAsync(TradeServer tradeServer);
        Task DeleteTradeServerAsync(TradeServer tradeServer);
    }
}