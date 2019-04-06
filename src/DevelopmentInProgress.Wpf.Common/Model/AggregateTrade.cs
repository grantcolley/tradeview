namespace DevelopmentInProgress.Wpf.Common.Model
{
    public class AggregateTrade : TradeBase
    {
        public string Symbol { get; set; }
        public bool IsBestPriceMatch { get; set; }
        public long FirstTradeId { get; set; }
        public long LastTradeId { get; set; }
    }
}