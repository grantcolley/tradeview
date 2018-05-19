using DevelopmentInProgress.MarketView.Interface.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevelopmentInProgress.MarketView.Test.Helper
{
    public static class TestHelper
    {
        private static string symbols;
        private static string symbolsStatistics;
        private static string orders;
        private static string eth;
        private static string ethStats;
        private static string trx;
        private static string trxStats;
        private static string accountInfo;
        private static string aggregateTrades;
        private static string aggregateTradesUpdated;
        private static string orderBook;
        private static string orderBookUpdated;

        static  TestHelper()
        {
            symbols = File.ReadAllText("Symbols.txt");
            symbolsStatistics = File.ReadAllText("SymbolsStatistics.txt");
            orders = File.ReadAllText("Orders.txt");
            accountInfo = File.ReadAllText("AccountInfo.txt");
            aggregateTrades = File.ReadAllText("AggregateTrades.txt");
            aggregateTradesUpdated = File.ReadAllText("AggregateTradesUpdated.txt");
            orderBook = File.ReadAllText("OrderBook.txt");
            orderBookUpdated = File.ReadAllText("OrderBookUpdated.txt");

            var e = Symbols.Single(s => s.BaseAsset.Symbol.Equals("ETH") && s.QuoteAsset.Symbol.Equals("BTC"));
            eth = JsonConvert.SerializeObject(e);

            var es = SymbolsStatistics.Single(s => s.Symbol.Equals("ETHBTC"));
            ethStats = JsonConvert.SerializeObject(es);

            var t = Symbols.Single(s => s.BaseAsset.Symbol.Equals("TRX") && s.QuoteAsset.Symbol.Equals("BTC"));
            trx = JsonConvert.SerializeObject(t);

            var ts = SymbolsStatistics.Single(s => s.Symbol.Equals("TRXBTC"));
            trxStats = JsonConvert.SerializeObject(ts);
        }
        
        public static List<Symbol> Symbols
        {
            get
            {
                return JsonConvert.DeserializeObject<List<Symbol>>(symbols);
            }
        }

        public static List<SymbolStats> SymbolsStatistics
        {
            get
            {
                return JsonConvert.DeserializeObject<List<SymbolStats>>(symbolsStatistics);
            }
        }

        public static List<Order> Orders
        {
            get
            {
                return JsonConvert.DeserializeObject<List<Order>>(orders);
            }
        }

        public static OrderBook OrderBook
        {
            get
            {
                return JsonConvert.DeserializeObject<OrderBook>(orderBook);
            }
        }

        public static OrderBook OrderBookUpdated
        {
            get
            {
                return JsonConvert.DeserializeObject<OrderBook>(orderBookUpdated);
            }
        }

        public static List<AggregateTrade> AggregateTrades
        {
            get
            {
                return JsonConvert.DeserializeObject<List<AggregateTrade>>(aggregateTrades);
            }
        }

        public static List<AggregateTrade> AggregateTradesUpdated
        {
            get
            {
                return JsonConvert.DeserializeObject<List<AggregateTrade>>(aggregateTradesUpdated);
            }
        }

        public static AccountInfo AccountInfo
        {
            get
            {
                return JsonConvert.DeserializeObject<AccountInfo>(accountInfo);
            }
        }

        public static Symbol Trx
        {
            get
            {
                return JsonConvert.DeserializeObject<Symbol>(trx);
            }
        }

        public static SymbolStats TrxStats
        {
            get
            {
                return JsonConvert.DeserializeObject<SymbolStats>(trxStats);
            }
        }

        public static Symbol Eth
        {
            get
            {
                return JsonConvert.DeserializeObject<Symbol>(eth);
            }
        }

        public static SymbolStats EthStats
        {
            get
            {
                return JsonConvert.DeserializeObject<SymbolStats>(ethStats);
            }
        }

        public static SymbolStats EthStats_UpdatedLastPrice_Upwards
        {
            get
            {
                var origEthStats = EthStats;
                var updatedEthStats = EthStats;

                updatedEthStats.PriceChange = 0.00156M;
                updatedEthStats.LastPrice = origEthStats.LastPrice + updatedEthStats.PriceChange;

                return updatedEthStats;
            }
        }

        public static SymbolStats EthStats_UpdatedLastPrice_Downwards
        {
            get
            {
                var origEthStats = EthStats;
                var updatedEthStats = EthStats;

                updatedEthStats.PriceChange = 0.00156M;
                updatedEthStats.LastPrice = origEthStats.LastPrice - updatedEthStats.PriceChange;

                return updatedEthStats;
            }
        }
    }
}
