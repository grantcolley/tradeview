using Newtonsoft.Json;
using System;

namespace DevelopmentInProgress.TradeView.Interface.Server
{
    public class ServerNotification
    {
        public string Machine { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }
    }
}
