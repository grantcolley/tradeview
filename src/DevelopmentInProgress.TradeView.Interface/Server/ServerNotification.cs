using System;

namespace DevelopmentInProgress.TradeView.Interface.Server
{
    public class ServerNotification
    {
        public string Machine { get; set; }
        public string Message { get; set; }
        public string MethodName { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
