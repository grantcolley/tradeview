using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System;

namespace DevelopmentInProgress.TradeView.Interface.Model
{
    public class Trade : ITrade
    {
        public string Symbol { get; set; }
        public Exchange Exchange { get; set; }
        public long Id { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public long BuyerOrderId { get; set; }
        public long SellerOrderId { get; set; }
        public DateTime Time { get; set; }
        public bool IsBuyerMaker { get; set; }
        public bool IsBestPriceMatch { get; set; }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is Trade))
            {
                return false;
            }
            else
            {
                return (Id == ((Trade)obj).Id);
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
