using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;
using System.Collections.ObjectModel;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Cache
{
    public interface IServerMonitorCache : IDisposable
    {
        void StartObservingServers();
        ObservableCollection<ServerMonitor> GetServerMonitors();
        event EventHandler<ServerMonitorCacheEventArgs> ServerMonitorCacheNotification;
    }
}