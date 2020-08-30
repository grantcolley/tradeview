using DevelopmentInProgress.TradeView.Core.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Server
{
    public class ServerMonitor : TradeServer, IServerMonitor
    {
        public ServerMonitor()
        {
            Strategies = new List<ServerStrategy>();
        }

        public string StartedBy { get; set; }
        public string StoppedBy { get; set; }
        public DateTime Started { get; set; }
        public DateTime Stopped { get; set; }
        public List<ServerStrategy> Strategies { get; private set; }

        public ServerNotification GetServerNotification(List<ServerStrategy> strategies)
        {
            return this.GetNotification(strategies);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }
    }
}
