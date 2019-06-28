using DevelopmentInProgress.MarketView.Interface.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Events
{
    public class CandlestickEventArgs
    {
        public IEnumerable<Candlestick> Candlesticks { get; set; }
    }
}