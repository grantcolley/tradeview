using DevelopmentInProgress.MarketView.Interface.Enums;

namespace DevelopmentInProgress.MarketView.Interface.Model
{
    public class User
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string ApiPassPhrase { get; set; }
        public Exchange Exchange { get; set; }
        public RateLimiter RateLimiter { get; set; }
    }
}