using DevelopmentInProgress.TradeView.Wpf.Dashboard.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.Services
{
    public interface IDashboardService
    {
        Task<List<ServerMonitor>> GetServers();
    }
}
