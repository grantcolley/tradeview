using DevelopmentInProgress.TradeView.Interface.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Events
{
    public class CandlestickEventArgs
    {
        public IEnumerable<Candlestick> Candlesticks { get; set; }
    }
}