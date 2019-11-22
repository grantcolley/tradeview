using DevelopmentInProgress.TradeView.Interface.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Events
{
    public class StatisticsEventArgs
    {
        public IEnumerable<SymbolStats> Statistics { get; set; }
    }
}