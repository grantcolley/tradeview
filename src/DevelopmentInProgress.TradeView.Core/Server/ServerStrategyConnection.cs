using System;

namespace DevelopmentInProgress.TradeView.Core.Server
{
    public class ServerStrategyConnection
    {
        public string Connection { get; set; }
        public DateTime Connected { get; set; }
        public DateTime Disconnected { get; set; }
    }
}
