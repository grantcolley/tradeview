using DevelopmentInProgress.TradeView.Interface.Enums;

namespace DevelopmentInProgress.TradeView.Interface.Model
{
    public class User
    {
        public string AccountName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string ApiPassPhrase { get; set; }
        public Exchange Exchange { get; set; }
        public RateLimiter RateLimiter { get; set; }
    }
}