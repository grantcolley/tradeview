namespace DevelopmentInProgress.MarketView.Interface.Model
{
    public class ClientOrder
    {
        public string Symbol { get; set; }

        public OrderType Type { get; set; }

        public OrderSide Side { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal IcebergQuantity { get; set; }

        public TimeInForce TimeInForce { get; set; }

        public decimal StopPrice { get; set; }

        public AccountBalance BaseAccountBalance { get; set; }

        public AccountBalance QuoteAccountBalance { get; set; }
    }
}
