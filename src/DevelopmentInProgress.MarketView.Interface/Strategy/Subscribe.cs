using System;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    [Flags]
    public enum Subscribe
    {
        None = 0,
        AccountInfo = 1,
        Trades = 2,
        OrderBook = 4,
        Statistics = 8
    }
}
