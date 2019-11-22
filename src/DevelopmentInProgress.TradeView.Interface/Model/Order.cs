using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Model
{
    public class Order
    {
        public User User { get; set; }
        public string Symbol { get; set; }
        public string Id { get; set; }
        public string ClientOrderId { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalQuantity { get; set; }
        public decimal ExecutedQuantity { get; set; }
        public OrderStatus Status { get; set; }
        public TimeInForce TimeInForce { get; set; }
        public OrderType Type { get; set; }
        public OrderSide Side { get; set; }
        public decimal StopPrice { get; set; }
        public decimal IcebergQuantity { get; set; }
        public DateTime Time { get; set; }
        public bool IsWorking { get; set; }
        public IEnumerable<Fill> Fills { get; set; }
    }
}