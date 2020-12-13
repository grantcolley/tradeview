using DevelopmentInProgress.TradeView.Core.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers.Data
{
    public static class TestDataHelper
    {
        private readonly static string symbols;
        private readonly static string symbolsStatistics;
        private readonly static string orders;
        private readonly static string accountInfo;
        private readonly static string aggregateTrades;
        private readonly static string aggregateTradesUpdated;
        private readonly static string orderBook;
        private readonly static string orderBookUpdated;

        static TestDataHelper()
        {
            symbols = File.ReadAllText(@"..\netcoreapp2.0\Helpers\Data\Symbols.txt");
            symbolsStatistics = File.ReadAllText(@"..\netcoreapp2.0\Helpers\Data\SymbolsStatistics.txt");
            orders = File.ReadAllText(@"..\netcoreapp2.0\Helpers\Data\Orders.txt");
            accountInfo = File.ReadAllText(@"..\netcoreapp2.0\Helpers\Data\AccountInfo.txt");
            aggregateTrades = File.ReadAllText(@"..\netcoreapp2.0\Helpers\Data\AggregateTrades.txt");
            aggregateTradesUpdated = File.ReadAllText(@"..\netcoreapp2.0\Helpers\Data\AggregateTradesUpdated.txt");
            orderBook = File.ReadAllText(@"..\netcoreapp2.0\Helpers\Data\OrderBook.txt");
            orderBookUpdated = File.ReadAllText(@"..\netcoreapp2.0\Helpers\Data\OrderBookUpdated.txt");
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

        public static OrderBook GetOrderBook(string symbol)
        {
            var orders = JsonConvert.DeserializeObject<OrderBook>(orderBook);
            orders.Symbol = symbol;
            return orders;
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

        public static List<AggregateTrade> GetAggregateTradesUpdated(string symbol)
        {
            var trades = JsonConvert.DeserializeObject<List<AggregateTrade>>(aggregateTradesUpdated);
            trades.ForEach(t => t.Symbol = symbol);
            return trades;
        }

        public static AccountInfo AccountInfo
        {
            get
            {
                return JsonConvert.DeserializeObject<AccountInfo>(accountInfo);
            }
        }
    }
}
