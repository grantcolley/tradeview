using DevelopmentInProgress.TradeView.Core.Interfaces;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Events
{
    public class TradeEventArgs
    {
        public IEnumerable<ITrade> Trades { get; set; }
    }
}