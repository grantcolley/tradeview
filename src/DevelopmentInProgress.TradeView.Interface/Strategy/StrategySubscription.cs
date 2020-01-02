using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Model;

namespace DevelopmentInProgress.TradeView.Interface.Strategy
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
        public Subscribe Subscribe { get; set; }
        public CandlestickInterval CandlestickInterval { get; set; }
    }
}