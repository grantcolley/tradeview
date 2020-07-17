using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public static class ServerMonitorHelper
    {
        public static void UpdateServerMonitor(ServerMonitor serverMonitor, Core.Server.ServerMonitor ism)
        {
            if (serverMonitor == null)
            {
                throw new ArgumentNullException(nameof(serverMonitor));
            }

            if (ism == null)
            {
                throw new ArgumentNullException(nameof(ism));
            }

            serverMonitor.Started = ism.Started;
            serverMonitor.StartedBy = ism.StartedBy;

            RemoveOldStrategies(serverMonitor.Strategies, ism.Strategies);

            UpdateStrategies(serverMonitor.Strategies, ism.Strategies);

            AddStrategies(serverMonitor.Strategies, ism.Strategies);

            serverMonitor.StrategyCount = serverMonitor.Strategies.Count;
        }

        private static void RemoveOldStrategies(ObservableCollection<ServerStrategy> strategies, List<Core.Server.ServerStrategy> serverStrategies)
        {
            var removeStrategies = strategies.Where(s => !serverStrategies.Any(ism => ism.Strategy.Name == s.Name)).ToList();
            foreach (var strategy in removeStrategies)
            {
                strategies.Remove(strategy);
            }
        }

        private static void UpdateStrategies(ObservableCollection<ServerStrategy> strategies, List<Core.Server.ServerStrategy> serverStrategies)
        {
            Func<ServerStrategy, Core.Server.ServerStrategy, ServerStrategy> updateStrategy = ((s, ism) =>
            {
                s.Started = ism.Started;
                s.StartedBy = ism.StartedBy;
                s.Parameters = ism.Strategy.Parameters;

                var removeConnections = s.Connections.Where(c => !ism.Connections.Any(ismc => ismc.Connection == c.Name)).ToList();
                foreach (var connection in removeConnections)
                {
                    s.Connections.Remove(connection);
                }

                var newServerConnections = ism.Connections.Where(ismc => !s.Connections.Any(c => c.Name == ismc.Connection)).ToList();
                var newConnections = newServerConnections.Select(ismc => new Connection
                {
                    Name = ismc.Connection,
                    Connected = ismc.Connected
                });

                foreach (var connection in newConnections)
                {
                    s.Connections.Add(connection);
                }

                s.ConnectionCount = s.Connections.Count;

                return s;
            });

            (from s in strategies
             join ism in serverStrategies
             on s.Name equals ism.Strategy.Name
             select updateStrategy(s, ism)).ToList();
        }

        private static void AddStrategies(ObservableCollection<ServerStrategy> strategies, List<Core.Server.ServerStrategy> serverStrategies)
        {
            var newServerStrategies = serverStrategies.Where(ism => !strategies.Any(s => s.Name == ism.Strategy.Name)).ToList();

            Func<Core.Server.ServerStrategy, ServerStrategy> f = (ism) =>
            {
                var serverStrategy = new ServerStrategy
                {
                    Name = ism.Strategy.Name,
                    Started = ism.Started,
                    StartedBy = ism.StartedBy,
                    Parameters = ism.Strategy.Parameters,
                    ConnectionCount = ism.Connections.Count
                };

                var connections = ism.Connections.Select(c => new Connection
                {
                    Name = c.Connection,
                    Connected = c.Connected
                }).ToList();

                connections.ForEach(serverStrategy.Connections.Add);

                return serverStrategy;
            };

            var newStrategies = newServerStrategies.Select(ism => f(ism));

            foreach(var strategy in newStrategies)
            {
                strategies.Add(strategy);
            }
        }
    }
}
