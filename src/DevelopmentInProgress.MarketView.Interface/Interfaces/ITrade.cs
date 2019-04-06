using System;

namespace DevelopmentInProgress.MarketView.Interface.Interfaces
{
    public interface ITrade
    {
        long Id { get; set; }
        decimal Price { get; set; }
        decimal Quantity { get; set; }
        DateTime Time { get; set; }
        bool IsBuyerMaker { get; set; }
    }
}
