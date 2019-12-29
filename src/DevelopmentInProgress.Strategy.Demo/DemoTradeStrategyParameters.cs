using DevelopmentInProgress.TradeView.Interface.Strategy;

namespace DevelopmentInProgress.Strategy.Demo
{
    public class DemoTradeStrategyParameters : StrategyParameters
    {
        public decimal BuyIndicator { get; set; }
        public decimal SellIndicator { get; set; }
        public int TradeMovingAvarageSetLength { get; set; }
    }
}