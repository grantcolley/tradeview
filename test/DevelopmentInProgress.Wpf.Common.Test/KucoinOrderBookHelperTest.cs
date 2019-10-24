using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.Common.Helpers;
using DevelopmentInProgress.Wpf.Common.Model;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using System.Threading.Tasks;
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
            var kucoinExchangeApi = new KucoinExchangeTestApi(true);
            var kucoinOrderBookHelper = new KucoinOrderBookHelper(kucoinExchangeApi);
            var subscribeOrderBookUpdate = TestHelper.KucoinOrderBook_15_18;
            var symbol = new Symbol { BaseAsset = new Interface.Asset { Symbol = "BTC" }, QuoteAsset = new Interface.Asset { Symbol = "USDT" } };

            // Act
            var orderBook = kucoinOrderBookHelper.CreateLocalOrderBook(symbol, subscribeOrderBookUpdate, 10, 10);

            // Assert
            Assert.AreEqual(orderBook.LastUpdateId, 18);

            var asks = new List<Interface.OrderBookPriceLevel>(subscribeOrderBookUpdate.Asks);
            Assert.AreEqual(asks[0].Price, 3988.59m);
            Assert.AreEqual(asks[0].Quantity, 3);
            Assert.AreEqual(asks[1].Price, 3988.60m);
            Assert.AreEqual(asks[1].Quantity, 47);
            Assert.AreEqual(asks[2].Price, 3988.62m);
            Assert.AreEqual(asks[2].Quantity, 8);

            var bids = new List<Interface.OrderBookPriceLevel>(subscribeOrderBookUpdate.Bids);
            Assert.AreEqual(bids[0].Price, 3988.51m);
            Assert.AreEqual(bids[0].Quantity, 56);
            Assert.AreEqual(bids[1].Price, 3988.50m);
            Assert.AreEqual(bids[1].Quantity, 44);
            Assert.AreEqual(bids[2].Price, 3988.49m);
            Assert.AreEqual(bids[2].Quantity, 100);
            Assert.AreEqual(bids[3].Price, 3988.48m);
            Assert.AreEqual(bids[3].Quantity, 10);
        }

        [TestMethod]
        public void CreateLocalOrderBook_ReplayCache_IUIIRA()
        {
            // Arrange
            var kucoinExchangeApi = new KucoinExchangeTestApi();
            var kucoinOrderBookHelper = new KucoinOrderBookHelper(kucoinExchangeApi);
            var subscribeOrderBookUpdate = TestHelper.KucoinOrderBook_IUIIRA;
            var symbol = new Symbol { BaseAsset = new Interface.Asset { Symbol = "BTC" }, QuoteAsset = new Interface.Asset { Symbol = "USDT" } };

            // Act
            var orderBook = kucoinOrderBookHelper.CreateLocalOrderBook(symbol, subscribeOrderBookUpdate, 10, 10);

            // Assert
            Assert.AreEqual(orderBook.LastUpdateId, 112);

            var asks = new List<Interface.OrderBookPriceLevel>(subscribeOrderBookUpdate.Asks);
            Assert.AreEqual(asks[0].Price, 123.19m);
            Assert.AreEqual(asks[0].Quantity, 19);
            Assert.AreEqual(asks[1].Price, 123.20m);
            Assert.AreEqual(asks[1].Quantity, 20);
            Assert.AreEqual(asks[2].Price, 123.21m);
            Assert.AreEqual(asks[2].Quantity, 211);
            Assert.AreEqual(asks[3].Price, 123.22m);
            Assert.AreEqual(asks[3].Quantity, 22);
            Assert.AreEqual(asks[4].Price, 123.23m);
            Assert.AreEqual(asks[4].Quantity, 23);
            Assert.AreEqual(asks[5].Price, 123.24m);
            Assert.AreEqual(asks[5].Quantity, 24);
            Assert.AreEqual(asks[6].Price, 123.26m);
            Assert.AreEqual(asks[6].Quantity, 26);
            Assert.AreEqual(asks[7].Price, 123.27m);
            Assert.AreEqual(asks[7].Quantity, 27);
            Assert.AreEqual(asks[8].Price, 123.28m);
            Assert.AreEqual(asks[8].Quantity, 28);

            var bids = new List<Interface.OrderBookPriceLevel>(subscribeOrderBookUpdate.Bids);
            Assert.AreEqual(bids[0].Price, 123.18m);
            Assert.AreEqual(bids[0].Quantity, 18);
            Assert.AreEqual(bids[1].Price, 123.17m);
            Assert.AreEqual(bids[1].Quantity, 17);
            Assert.AreEqual(bids[2].Price, 123.16m);
            Assert.AreEqual(bids[2].Quantity, 160);
            Assert.AreEqual(bids[3].Price, 123.15m);
            Assert.AreEqual(bids[3].Quantity, 15);
            Assert.AreEqual(bids[4].Price, 123.14m);
            Assert.AreEqual(bids[4].Quantity, 14);
            Assert.AreEqual(bids[5].Price, 123.13m);
            Assert.AreEqual(bids[5].Quantity, 13);
            Assert.AreEqual(bids[6].Price, 123.11m);
            Assert.AreEqual(bids[6].Quantity, 11);
            Assert.AreEqual(bids[7].Price, 123.10m);
            Assert.AreEqual(bids[7].Quantity, 10);
            Assert.AreEqual(bids[8].Price, 123.09m);
            Assert.AreEqual(bids[8].Quantity, 09);
        }

        [TestMethod]
        public void CreateLocalOrderBook_ReplayCache_RUIRRA()
        {
            // Arrange
            var kucoinExchangeApi = new KucoinExchangeTestApi();
            var kucoinOrderBookHelper = new KucoinOrderBookHelper(kucoinExchangeApi);
            var subscribeOrderBookUpdate = TestHelper.KucoinOrderBook_RUIRRA;
            var symbol = new Symbol { BaseAsset = new Interface.Asset { Symbol = "BTC" }, QuoteAsset = new Interface.Asset { Symbol = "USDT" } };

            // Act
            var orderBook = kucoinOrderBookHelper.CreateLocalOrderBook(symbol, subscribeOrderBookUpdate, 10, 10);

            // Assert
            Assert.AreEqual(orderBook.LastUpdateId, 112);

            var asks = new List<Interface.OrderBookPriceLevel>(subscribeOrderBookUpdate.Asks);
            Assert.AreEqual(asks[0].Price, 123.21m);
            Assert.AreEqual(asks[0].Quantity, 211);
            Assert.AreEqual(asks[1].Price, 123.22m);
            Assert.AreEqual(asks[1].Quantity, 22);
            Assert.AreEqual(asks[2].Price, 123.26m);
            Assert.AreEqual(asks[2].Quantity, 26);
            Assert.AreEqual(asks[3].Price, 123.27m);
            Assert.AreEqual(asks[3].Quantity, 27);
            Assert.AreEqual(asks[4].Price, 123.28m);
            Assert.AreEqual(asks[4].Quantity, 28);

            var bids = new List<Interface.OrderBookPriceLevel>(subscribeOrderBookUpdate.Bids);
            Assert.AreEqual(bids[0].Price, 123.16m);
            Assert.AreEqual(bids[0].Quantity, 160);
            Assert.AreEqual(bids[1].Price, 123.15m);
            Assert.AreEqual(bids[1].Quantity, 15);
            Assert.AreEqual(bids[2].Price, 123.11m);
            Assert.AreEqual(bids[2].Quantity, 11);
            Assert.AreEqual(bids[3].Price, 123.10m);
            Assert.AreEqual(bids[3].Quantity, 10);
            Assert.AreEqual(bids[4].Price, 123.09m);
            Assert.AreEqual(bids[4].Quantity, 09);
        }
    }
}
