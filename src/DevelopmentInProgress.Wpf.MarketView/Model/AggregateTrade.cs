using System;

namespace DevelopmentInProgress.Wpf.MarketView.Model
{
    public class AggregateTrade : EntityBase
    {
        private decimal price;
        private decimal quantity;
        private DateTime time;

        public string Symbol { get; set; }
        public new long Id { get; set; }
        public bool IsBuyerMaker { get; set; }
        public bool IsBestPriceMatch { get; set; }
        public long FirstTradeId { get; set; }
        public long LastTradeId { get; set; }

        public decimal Price
        {
            get { return price; }
            set
            {
                if (price != value)
                {
                    price = value;
                    OnPropertyChanged("Price");
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
                    OnPropertyChanged("Quantity");
                }
            }
        }

        public DateTime Time
        {
            get { return time; }
            set
            {
                if (time != value)
                {
                    time = value;
                    OnPropertyChanged("Time");
                }
            }
        }
    }
}