using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Services
{
    public interface ITradeServerService
    {
        Task<List<TradeServer>> GetTradeServers();
        Task<TradeServer> GetTradeServer(string tradeServerName);
        Task SaveTradeServer(TradeServer tradeServer);
        Task DeleteTradeServer(TradeServer tradeServer);
    }
}