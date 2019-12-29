using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System;

namespace DevelopmentInProgress.Strategy.Demo
{
    public class DemoTrade : ITrade
    {
        public string Symbol { get; set; }
        public long Id { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public DateTime Time { get; set; }
        public bool IsBuyerMaker { get; set; }
        public bool IsBestPriceMatch { get; set; }
        
        public decimal SmaPrice { get; set; }
        public decimal BuyIndicatorPrice { get; set; }
        public decimal SellIndicatorPrice { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is DemoTrade))
            {
                return false;
            }
            else
            {
                return (Id == ((DemoTrade)obj).Id);
            }
        }

        public override int GetHashCode()
        {
            return (int)Id;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name} {Id}";
        }
    }
}
