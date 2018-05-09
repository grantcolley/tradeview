using DevelopmentInProgress.MarketView.Interface.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevelopmentInProgress.MarketView.Interface.Test
{
    public static class MarketHelper
    {
        static  MarketHelper()
        {
            var symbols = File.ReadAllText("Symbols.txt");
            var symbolsStatistics = File.ReadAllText("SymbolsStatistics.txt");

            Symbols = JsonConvert.DeserializeObject<List<Symbol>>(symbols);
            SymbolsStatistics = JsonConvert.DeserializeObject<List<SymbolStats>>(symbolsStatistics);

            Eth = Symbols.Single(s => s.BaseAsset.Symbol.Equals("ETH") && s.QuoteAsset.Symbol.Equals("BTC"));
            EthStats = SymbolsStatistics.Single(s => s.Symbol.Equals("ETHBTC"));

            Trx = Symbols.Single(s => s.BaseAsset.Symbol.Equals("TRX") && s.QuoteAsset.Symbol.Equals("BTC"));
            TrxStats = SymbolsStatistics.Single(s => s.Symbol.Equals("TRXBTC"));
        }

        public static List<Symbol> Symbols { get; private set; }
        public static List<SymbolStats> SymbolsStatistics { get; private set; }
        public static Symbol Trx { get; private set; }
        public static SymbolStats TrxStats { get; private set; }
        public static Symbol Eth { get; private set; }
        public static SymbolStats EthStats { get; private set; }
    }
}
