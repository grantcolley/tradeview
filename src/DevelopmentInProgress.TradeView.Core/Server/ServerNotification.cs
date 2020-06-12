using Newtonsoft.Json;
using System;

namespace DevelopmentInProgress.TradeView.Core.Server
{
    public class ServerNotification
    {
        public string Machine { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public ServerNotificationLevel NotificationLevel { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }
    }
}
