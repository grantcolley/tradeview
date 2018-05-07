using DevelopmentInProgress.MarketView.Interface.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DevelopmentInProgress.MarketView.Interface.Test
{
    public class MarketHelper
    {
        public MarketHelper()
        {
            var symbols = File.ReadAllText("Symbols.txt");
            var symbolsStatistics = File.ReadAllText("SymbolsStatistics.txt");

            Symbols = JsonConvert.DeserializeObject<List<Symbol>>(symbols);
            SymbolsStatistics = JsonConvert.DeserializeObject<List<SymbolStats>>(symbolsStatistics);
        }

        public List<Symbol> Symbols { get; private set; }
        public List<SymbolStats> SymbolsStatistics { get; private set; }
    }
}
