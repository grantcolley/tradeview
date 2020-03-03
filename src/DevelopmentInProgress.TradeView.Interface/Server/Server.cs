using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Server
{
    public class Server : IServer
    {
        public Server()
        {
            Strategies = new List<ServerStrategy>();
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public int MaxDegreeOfParallelism { get; set; } = 5;
        public string StartedBy { get; set; } = Environment.UserName;
        public string StoppedBy { get; set; }
        public DateTime Started { get; set; } = DateTime.Now;
        public DateTime Stopped { get; set; }
        public List<ServerStrategy> Strategies { get; set; }
    }
}
