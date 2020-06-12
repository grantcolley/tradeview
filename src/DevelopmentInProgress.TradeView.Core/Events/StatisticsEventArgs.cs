using DevelopmentInProgress.TradeView.Core.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Events
{
    public class StatisticsEventArgs
    {
        public IEnumerable<SymbolStats> Statistics { get; set; }
    }
}