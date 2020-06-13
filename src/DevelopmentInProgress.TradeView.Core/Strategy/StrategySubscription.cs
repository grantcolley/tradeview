using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Model;

namespace DevelopmentInProgress.TradeView.Core.Strategy
{
    public class StrategySubscription
    {
        public string AccountName { get; set; }
        public string Symbol { get; set; }
        public int Limit { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string ApiPassPhrase { get; set; }
        public Exchange Exchange { get; set; }
        public Subscribes Subscribes { get; set; }
        public CandlestickInterval CandlestickInterval { get; set; }
    }
}