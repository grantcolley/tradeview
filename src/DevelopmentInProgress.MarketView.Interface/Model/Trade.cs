using DevelopmentInProgress.MarketView.Interface.Interfaces;
using System;

namespace DevelopmentInProgress.MarketView.Interface.Model
{
    public class Trade : ITrade
    {
        public string Symbol { get; set; }
        public long Id { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public long BuyerOrderId { get; set; }
        public long SellerOrderId { get; set; }
        public DateTime Time { get; set; }
        public bool IsBuyerMaker { get; set; }
        public bool IsBestPriceMatch { get; set; }
    }
}
