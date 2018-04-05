namespace DevelopmentInProgress.MarketView.Interface.Model
{
    public class Fill
    {
        public decimal Price { get; }
        public decimal Quantity { get; }
        public decimal Commission { get; }
        public string CommissionAsset { get; }
        public long TradeId { get; }
    }
}
