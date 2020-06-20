using System;

namespace DevelopmentInProgress.TradeView.Core.Server
{
    public class TradeServer : ITradeServer
    {
        public string Name { get; set; }
        public Uri Uri { get; set; }
        public int MaxDegreeOfParallelism { get; set; }
        public bool Enabled { get; set; }
    }
}
