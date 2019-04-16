namespace DevelopmentInProgress.Wpf.Common.Model
{
    public class OrderBookTop : EntityBase
    {
        public OrderBookPriceLevel Bid { get; set; }
        public OrderBookPriceLevel Ask { get; set; }
    }
}
