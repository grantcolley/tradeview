using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using System;

namespace DevelopmentInProgress.TradeView.Core.Model
{
    public class AggregateTrade : ITrade
    {
        public string Symbol { get; set; }
        public Exchange Exchange { get; set; }
        public long Id { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public DateTime Time { get; set; }
        public bool IsBuyerMaker { get; set; }
        public bool IsBestPriceMatch { get; set; }
        public long FirstTradeId { get; set; }
        public long LastTradeId { get; set; }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is AggregateTrade))
            {
                return false;
            }
            else
            {
                return (Id == ((AggregateTrade)obj).Id);
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
