using DevelopmentInProgress.TradeView.Core.Strategy;

namespace DevelopmentInProgress.Strategy.Common.Parameter
{
    public class MovingAverageTradeParameters : StrategyParameters
    {
        public int TradeRange { get; set; }
        public decimal BuyIndicator { get; set; }
        public decimal SellIndicator { get; set; }
        public int MovingAvarageRange { get; set; }
    }
}
