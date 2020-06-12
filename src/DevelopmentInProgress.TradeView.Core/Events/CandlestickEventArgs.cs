using DevelopmentInProgress.TradeView.Core.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Events
{
    public class CandlestickEventArgs
    {
        public IEnumerable<Candlestick> Candlesticks { get; set; }
    }
}