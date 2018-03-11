namespace DevelopmentInProgress.MarketView.Interface.Model
{
    public class OrderBookTop
    {
        public string Symbol { get; set; }
        public OrderBookPriceLevel Bid { get; set; }
        public OrderBookPriceLevel Ask { get; set; }
    }
}
