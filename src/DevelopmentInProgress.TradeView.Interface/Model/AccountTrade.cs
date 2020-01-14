using System;
using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Interfaces;

namespace DevelopmentInProgress.TradeView.Interface.Model
{
    public class AccountTrade : ITrade
    {
        public string Symbol { get; set; }
        public Exchange Exchange { get; set; }
        public long Id { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public DateTime Time { get; set; }
        public bool IsBuyerMaker { get; set; }
        public bool IsBestPriceMatch { get; set; }
        public long BuyerOrderId { get; set; }
        public long SellerOrderId { get; set; }
        public long OrderId { get; set; }
        public decimal QuoteQuantity { get; set; }
        public decimal Commission { get; set; }
        public string CommissionAsset { get; set; }
        public bool IsBuyer { get; set; }
        public bool IsMaker { get; set; }

        public AccountTrade Clone()
        {
            return MemberwiseClone() as AccountTrade;
        }
    }
}