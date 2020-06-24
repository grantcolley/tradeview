using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Cache
{
    public interface IServerMonitorCache : IDisposable
    {
        Task RefreshServerMonitorsAsync();
        Task<ObservableCollection<ServerMonitor>> GetServerMonitorsAsync();
        event EventHandler<ServerMonitorCacheEventArgs> ServerMonitorCacheNotification;
    }
}