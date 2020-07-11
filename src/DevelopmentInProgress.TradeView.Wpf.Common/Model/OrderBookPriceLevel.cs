namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class OrderBookPriceLevel : EntityBase
    {
        private decimal price;
        private decimal quantity;

        public decimal Price
        {
            get { return price; }
            set
            {
                if (price != value)
                {
                    price = value;
                    OnPropertyChanged(nameof(Price));
                }
            }
        }

        public decimal Quantity
        {
            get { return quantity; }
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }
    }
}