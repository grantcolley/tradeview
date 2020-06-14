using System;

namespace DevelopmentInProgress.TradeView.Core.TradeStrategy
{
    [Flags]
    public enum Subscribes
    {
        None = 0,
        AccountInfo = 1,
        Trades = 2,
        OrderBook = 4,
        Candlesticks = 8
    }
}
