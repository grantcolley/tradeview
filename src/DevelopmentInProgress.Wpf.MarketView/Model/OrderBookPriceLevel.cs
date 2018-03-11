using System.Collections;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.MarketView.Model
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

        public static IComparer<OrderBookPriceLevel> SortByPriceAsc()
        {
            return new ComparePriceAsc();
        }

        public static IComparer<OrderBookPriceLevel> CompareByPriceDesc()
        {
            return new ComparePriceAsc();
        }

        private class ComparePriceAsc : IComparer<OrderBookPriceLevel>
        {
            public int Compare(OrderBookPriceLevel x, OrderBookPriceLevel y)
            {
                return decimal.Compare(x.Price, y.Price);
            }
        }

        public class ComparePriceDesc : IComparer<OrderBookPriceLevel>
        {
            public int Compare(OrderBookPriceLevel x, OrderBookPriceLevel y)
            {
                if (y.Price < x.Price)
                {
                    return -1;
                }
                else if (y.Price == x.Price)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}