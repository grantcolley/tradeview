using System;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    public class StrategyPerformanceIndicator
    {
        public string StrategyName { get; set; }
        public double SessionId { get; set; }
        public DateTime Date { get; set; }
        public decimal Quantity { get; set; }
        public decimal Performance { get; set; }
    }
}