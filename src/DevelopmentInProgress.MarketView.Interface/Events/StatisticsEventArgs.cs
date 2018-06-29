using DevelopmentInProgress.MarketView.Interface.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Events
{
    public class StatisticsEventArgs
    {
        public IEnumerable<SymbolStats> Statistics { get; set; }
    }
}