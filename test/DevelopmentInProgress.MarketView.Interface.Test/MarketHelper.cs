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
            var data = File.ReadAllText("Symbols.txt");
            Symbols = JsonConvert.DeserializeObject<List<Symbol>>(data);
        }

        public List<Symbol> Symbols { get; private set; }
    }
}
