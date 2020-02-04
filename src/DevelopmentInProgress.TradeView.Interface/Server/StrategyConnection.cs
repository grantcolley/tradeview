using System;

namespace DevelopmentInProgress.TradeView.Interface.Server
{
    public class StrategyConnection
    {
        public string Connection { get; set; }
        public DateTime Connected { get; set; }
        public DateTime Disconnected { get; set; }
    }
}
