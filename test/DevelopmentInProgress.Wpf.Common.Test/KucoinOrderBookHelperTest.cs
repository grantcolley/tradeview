using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.Common.Helpers;
using DevelopmentInProgress.Wpf.Common.Model;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.Common.Test
{
    [TestClass]
    public class KucoinOrderBookHelperTest
    {
        [TestMethod]
        public void CreateLocalOrderBook_ReplayCache()
        {
            // Arrange
            var kucoinExchangeApi = new KucoinExchangeTestApi(KucoinExchangeTestApiEnum.KucoinApiExample);
            var kucoinOrderBookHelper = new KucoinOrderBookHelper(kucoinExchangeApi);
            var subscribeOrderBookUpdate = TestHelper.KucoinOrderBook_15_18;
            var symbol = new Symbol
            {
                BaseAsset = new Interface.Asset { Symbol = "BTC" },
                QuoteAsset = new Interface.Asset { Symbol = "USDT" },
                Price = new Interface.InclusiveRange { Increment = 0.00000001M },
                Quantity = new Interface.InclusiveRange { Increment = 1m }
            };

            // Act
            var orderBook = kucoinOrderBookHelper.CreateLocalOrderBook(symbol, subscribeOrderBookUpdate, 10, 10);

            // Assert
            Assert.AreEqual(orderBook.LastUpdateId, 18);

            Assert.AreEqual(orderBook.Asks[0].Price, 3988.59m);
            Assert.AreEqual(orderBook.Asks[0].Quantity, 3);
            Assert.AreEqual(orderBook.Asks[1].Price, 3988.60m);
            Assert.AreEqual(orderBook.Asks[1].Quantity, 47);
            Assert.AreEqual(orderBook.Asks[2].Price, 3988.62m);
            Assert.AreEqual(orderBook.Asks[2].Quantity, 8);

            Assert.AreEqual(orderBook.Bids[0].Price, 3988.48m);
            Assert.AreEqual(orderBook.Bids[0].Quantity, 10);
            Assert.AreEqual(orderBook.Bids[1].Price, 3988.49m);
            Assert.AreEqual(orderBook.Bids[1].Quantity, 100);
            Assert.AreEqual(orderBook.Bids[2].Price, 3988.50m);
            Assert.AreEqual(orderBook.Bids[2].Quantity, 44);
            Assert.AreEqual(orderBook.Bids[3].Price, 3988.51m);
            Assert.AreEqual(orderBook.Bids[3].Quantity, 56);
        }

        [TestMethod]
        public void CreateLocalOrderBook_ReplayCache_IUIIRA()
        {
            // Arrange
            var kucoinExchangeApi = new KucoinExchangeTestApi(KucoinExchangeTestApiEnum.CreateLocalOrderBookExample);
            var kucoinOrderBookHelper = new KucoinOrderBookHelper(kucoinExchangeApi);
            var subscribeOrderBookUpdate = TestHelper.KucoinOrderBook_Create_IUIIRA;
            var symbol = new Symbol
            {
                BaseAsset = new Interface.Asset { Symbol = "BTC" },
                QuoteAsset = new Interface.Asset { Symbol = "USDT" },
                Price = new Interface.InclusiveRange { Increment = 0.00000001M },
                Quantity = new Interface.InclusiveRange { Increment = 1m }
            };

            // Act
            var orderBook = kucoinOrderBookHelper.CreateLocalOrderBook(symbol, subscribeOrderBookUpdate, 10, 10);

            // Assert
            Assert.AreEqual(orderBook.LastUpdateId, 112);

            Assert.AreEqual(orderBook.Asks[0].Price, 123.19m);
            Assert.AreEqual(orderBook.Asks[0].Quantity, 19);
            Assert.AreEqual(orderBook.Asks[1].Price, 123.20m);
            Assert.AreEqual(orderBook.Asks[1].Quantity, 20);
            Assert.AreEqual(orderBook.Asks[2].Price, 123.21m);
            Assert.AreEqual(orderBook.Asks[2].Quantity, 211);
            Assert.AreEqual(orderBook.Asks[3].Price, 123.22m);
            Assert.AreEqual(orderBook.Asks[3].Quantity, 22);
            Assert.AreEqual(orderBook.Asks[4].Price, 123.23m);
            Assert.AreEqual(orderBook.Asks[4].Quantity, 23);
            Assert.AreEqual(orderBook.Asks[5].Price, 123.24m);
            Assert.AreEqual(orderBook.Asks[5].Quantity, 24);
            Assert.AreEqual(orderBook.Asks[6].Price, 123.26m);
            Assert.AreEqual(orderBook.Asks[6].Quantity, 26);
            Assert.AreEqual(orderBook.Asks[7].Price, 123.27m);
            Assert.AreEqual(orderBook.Asks[7].Quantity, 27);
            Assert.AreEqual(orderBook.Asks[8].Price, 123.28m);
            Assert.AreEqual(orderBook.Asks[8].Quantity, 28);

            Assert.AreEqual(orderBook.Bids[0].Price, 123.09m);
            Assert.AreEqual(orderBook.Bids[0].Quantity, 09);
            Assert.AreEqual(orderBook.Bids[1].Price, 123.10m);
            Assert.AreEqual(orderBook.Bids[1].Quantity, 10);
            Assert.AreEqual(orderBook.Bids[2].Price, 123.11m);
            Assert.AreEqual(orderBook.Bids[2].Quantity, 11);
            Assert.AreEqual(orderBook.Bids[3].Price, 123.13m);
            Assert.AreEqual(orderBook.Bids[3].Quantity, 13);
            Assert.AreEqual(orderBook.Bids[4].Price, 123.14m);
            Assert.AreEqual(orderBook.Bids[4].Quantity, 14);
            Assert.AreEqual(orderBook.Bids[5].Price, 123.15m);
            Assert.AreEqual(orderBook.Bids[5].Quantity, 15);
            Assert.AreEqual(orderBook.Bids[6].Price, 123.16m);
            Assert.AreEqual(orderBook.Bids[6].Quantity, 160);
            Assert.AreEqual(orderBook.Bids[7].Price, 123.17m);
            Assert.AreEqual(orderBook.Bids[7].Quantity, 17);
            Assert.AreEqual(orderBook.Bids[8].Price, 123.18m);
            Assert.AreEqual(orderBook.Bids[8].Quantity, 18);
        }

        [TestMethod]
        public void CreateLocalOrderBook_ReplayCache_RUIRRA()
        {
            // Arrange
            var kucoinExchangeApi = new KucoinExchangeTestApi(KucoinExchangeTestApiEnum.CreateLocalOrderBookExample);
            var kucoinOrderBookHelper = new KucoinOrderBookHelper(kucoinExchangeApi);
            var subscribeOrderBookUpdate = TestHelper.KucoinOrderBook_Create_RUIRRA;
            var symbol = new Symbol
            {
                BaseAsset = new Interface.Asset { Symbol = "BTC" },
                QuoteAsset = new Interface.Asset { Symbol = "USDT" },
                Price = new Interface.InclusiveRange { Increment = 0.00000001M },
                Quantity = new Interface.InclusiveRange { Increment = 1 }
            };

            // Act
            var orderBook = kucoinOrderBookHelper.CreateLocalOrderBook(symbol, subscribeOrderBookUpdate, 10, 10);

            // Assert
            Assert.AreEqual(orderBook.LastUpdateId, 112);

            Assert.AreEqual(orderBook.Asks[0].Price, 123.21m);
            Assert.AreEqual(orderBook.Asks[0].Quantity, 211);
            Assert.AreEqual(orderBook.Asks[1].Price, 123.22m);
            Assert.AreEqual(orderBook.Asks[1].Quantity, 22);
            Assert.AreEqual(orderBook.Asks[2].Price, 123.26m);
            Assert.AreEqual(orderBook.Asks[2].Quantity, 26);
            Assert.AreEqual(orderBook.Asks[3].Price, 123.27m);
            Assert.AreEqual(orderBook.Asks[3].Quantity, 27);
            Assert.AreEqual(orderBook.Asks[4].Price, 123.28m);
            Assert.AreEqual(orderBook.Asks[4].Quantity, 28);

            Assert.AreEqual(orderBook.Bids[0].Price, 123.09m);
            Assert.AreEqual(orderBook.Bids[0].Quantity, 09);
            Assert.AreEqual(orderBook.Bids[1].Price, 123.10m);
            Assert.AreEqual(orderBook.Bids[1].Quantity, 10);
            Assert.AreEqual(orderBook.Bids[2].Price, 123.11m);
            Assert.AreEqual(orderBook.Bids[2].Quantity, 11);
            Assert.AreEqual(orderBook.Bids[3].Price, 123.15m);
            Assert.AreEqual(orderBook.Bids[3].Quantity, 15);
            Assert.AreEqual(orderBook.Bids[4].Price, 123.16m);
            Assert.AreEqual(orderBook.Bids[4].Quantity, 160);
        }

        [TestMethod]
        public void UpdateLocalOrderBook()
        {
            // Arrange
            var kucoinExchangeApi = new KucoinExchangeTestApi(KucoinExchangeTestApiEnum.UpdateLocalOrderBookExample);
            var kucoinOrderBookHelper = new KucoinOrderBookHelper(kucoinExchangeApi);
            var subscribeOrderBookReplay = TestHelper.KucoinOrderBook_Update_RestReplay;
            var subscribeOrderBookUpdate = TestHelper.KucoinOrderBook_Update;
            var symbol = new Symbol
            {
                BaseAsset = new Interface.Asset { Symbol = "BTC" },
                QuoteAsset = new Interface.Asset { Symbol = "USDT" },
                Price = new Interface.InclusiveRange { Increment = 0.00000001M },
                Quantity = new Interface.InclusiveRange { Increment = 1 }
            };

            // Act
            var orderBook = kucoinOrderBookHelper.CreateLocalOrderBook(symbol, subscribeOrderBookReplay, 10, 10);

            kucoinOrderBookHelper.UpdateLocalOrderBook(orderBook, subscribeOrderBookUpdate, symbol.PricePrecision, symbol.QuantityPrecision, 10, 10);

            // Assert
            Assert.AreEqual(orderBook.LastUpdateId, 120);

            // Asks
            Assert.AreEqual(orderBook.Asks[0].Price, 0.15m);
            Assert.AreEqual(orderBook.Asks[0].Quantity, 15);
            Assert.AreEqual(orderBook.Asks[1].Price, 0.16m);
            Assert.AreEqual(orderBook.Asks[1].Quantity, 16);
            Assert.AreEqual(orderBook.Asks[2].Price, 0.17m);
            Assert.AreEqual(orderBook.Asks[2].Quantity, 17);
            Assert.AreEqual(orderBook.Asks[3].Price, 0.18m);
            Assert.AreEqual(orderBook.Asks[3].Quantity, 18);
            Assert.AreEqual(orderBook.Asks[4].Price, 0.19m);
            Assert.AreEqual(orderBook.Asks[4].Quantity, 19);

            // Bids
            Assert.AreEqual(orderBook.Bids[0].Price, 0.01m);
            Assert.AreEqual(orderBook.Bids[0].Quantity, 1);
            Assert.AreEqual(orderBook.Bids[1].Price, 0.02m);
            Assert.AreEqual(orderBook.Bids[1].Quantity, 2);
            Assert.AreEqual(orderBook.Bids[2].Price, 0.03m);
            Assert.AreEqual(orderBook.Bids[2].Quantity, 3);
            Assert.AreEqual(orderBook.Bids[3].Price, 0.04m);
            Assert.AreEqual(orderBook.Bids[3].Quantity, 4);
            Assert.AreEqual(orderBook.Bids[4].Price, 0.06m);
            Assert.AreEqual(orderBook.Bids[4].Quantity, 6);

            // TopAsks
            Assert.AreEqual(orderBook.TopAsks[0].Price, 0.19m);
            Assert.AreEqual(orderBook.TopAsks[0].Quantity, 19);
            Assert.AreEqual(orderBook.TopAsks[1].Price, 0.18m);
            Assert.AreEqual(orderBook.TopAsks[1].Quantity, 18);
            Assert.AreEqual(orderBook.TopAsks[2].Price, 0.17m);
            Assert.AreEqual(orderBook.TopAsks[2].Quantity, 17);
            Assert.AreEqual(orderBook.TopAsks[3].Price, 0.16m);
            Assert.AreEqual(orderBook.TopAsks[3].Quantity, 16);
            Assert.AreEqual(orderBook.TopAsks[4].Price, 0.15m);
            Assert.AreEqual(orderBook.TopAsks[4].Quantity, 15);

            // TopBids
            Assert.AreEqual(orderBook.TopBids[0].Price, 0.06m);
            Assert.AreEqual(orderBook.TopBids[0].Quantity, 6);
            Assert.AreEqual(orderBook.TopBids[1].Price, 0.04m);
            Assert.AreEqual(orderBook.TopBids[1].Quantity, 4);
            Assert.AreEqual(orderBook.TopBids[2].Price, 0.03m);
            Assert.AreEqual(orderBook.TopBids[2].Quantity, 3);
            Assert.AreEqual(orderBook.TopBids[3].Price, 0.02m);
            Assert.AreEqual(orderBook.TopBids[3].Quantity, 2);
            Assert.AreEqual(orderBook.TopBids[4].Price, 0.01m);
            Assert.AreEqual(orderBook.TopBids[4].Quantity, 1);

            // ChartAsks
            Assert.AreEqual(orderBook.ChartAsks[0].Price, 0.15m);
            Assert.AreEqual(orderBook.ChartAsks[0].Quantity, 15);
            Assert.AreEqual(orderBook.ChartAsks[1].Price, 0.16m);
            Assert.AreEqual(orderBook.ChartAsks[1].Quantity, 16);
            Assert.AreEqual(orderBook.ChartAsks[2].Price, 0.17m);
            Assert.AreEqual(orderBook.ChartAsks[2].Quantity, 17);
            Assert.AreEqual(orderBook.ChartAsks[3].Price, 0.18m);
            Assert.AreEqual(orderBook.ChartAsks[3].Quantity, 18);
            Assert.AreEqual(orderBook.ChartAsks[4].Price, 0.19m);
            Assert.AreEqual(orderBook.ChartAsks[4].Quantity, 19);

            // ChartBids
            Assert.AreEqual(orderBook.ChartBids[0].Price, 0.01m);
            Assert.AreEqual(orderBook.ChartBids[0].Quantity, 1);
            Assert.AreEqual(orderBook.ChartBids[1].Price, 0.02m);
            Assert.AreEqual(orderBook.ChartBids[1].Quantity, 2);
            Assert.AreEqual(orderBook.ChartBids[2].Price, 0.03m);
            Assert.AreEqual(orderBook.ChartBids[2].Quantity, 3);
            Assert.AreEqual(orderBook.ChartBids[3].Price, 0.04m);
            Assert.AreEqual(orderBook.ChartBids[3].Quantity, 4);
            Assert.AreEqual(orderBook.ChartBids[4].Price, 0.06m);
            Assert.AreEqual(orderBook.ChartBids[4].Quantity, 6);

            // ChartAggregatedAsks
            Assert.AreEqual(orderBook.ChartAggregatedAsks[0].Price, 0.15m);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[0].Quantity, 15);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[1].Price, 0.16m);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[1].Quantity, 31);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[2].Price, 0.17m);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[2].Quantity, 48);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[3].Price, 0.18m);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[3].Quantity, 66);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[4].Price, 0.19m);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[4].Quantity, 85);

            // ChartAggregatedBids
            Assert.AreEqual(orderBook.ChartAggregatedBids[0].Price, 0.01m);
            Assert.AreEqual(orderBook.ChartAggregatedBids[0].Quantity, 16);
            Assert.AreEqual(orderBook.ChartAggregatedBids[1].Price, 0.02m);
            Assert.AreEqual(orderBook.ChartAggregatedBids[1].Quantity, 15);
            Assert.AreEqual(orderBook.ChartAggregatedBids[2].Price, 0.03m);
            Assert.AreEqual(orderBook.ChartAggregatedBids[2].Quantity, 13);
            Assert.AreEqual(orderBook.ChartAggregatedBids[3].Price, 0.04m);
            Assert.AreEqual(orderBook.ChartAggregatedBids[3].Quantity, 10);
            Assert.AreEqual(orderBook.ChartAggregatedBids[4].Price, 0.06m);
            Assert.AreEqual(orderBook.ChartAggregatedBids[4].Quantity, 6);
        }
    }
}
