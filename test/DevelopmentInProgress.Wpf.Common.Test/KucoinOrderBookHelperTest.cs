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
            var orderBook = kucoinOrderBookHelper.CreateLocalOrderBookReplayCache(symbol, subscribeOrderBookUpdate, 10);

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
    }
}
