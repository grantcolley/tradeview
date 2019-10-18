using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.Common.Helpers;
using DevelopmentInProgress.Wpf.Common.Model;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.Common.Test
{
    [TestClass]
    public class KucoinOrderBookHelperTest
    {
        [TestMethod]
        public void CreateLocalOrderBook()
        {
            // Arrange
            var kucoinExchangeApi = new KucoinExchangeTestApi();
            var kucoinOrderBookHelper = new KucoinOrderBookHelper(kucoinExchangeApi);
            var subscribeOrderBookUpdate = TestHelper.KucoinOrderBook_15_18;
            var symbol = new Symbol { BaseAsset = new Interface.Asset { Symbol = "BTC" }, QuoteAsset = new Interface.Asset { Symbol = "USDT" } };

            // Act
            var orderBook = kucoinOrderBookHelper.CreateLocalOrderBook(symbol, subscribeOrderBookUpdate, 10);

            // Assert
            Assert.AreEqual(orderBook.LastUpdateId, 18);
            Assert.AreEqual(orderBook.TopAsks[0].Price, 3988.59);
            Assert.AreEqual(orderBook.TopAsks[0].Quantity, 3);
            Assert.AreEqual(orderBook.TopAsks[1].Price, 3988.60);
            Assert.AreEqual(orderBook.TopAsks[1].Quantity, 47);
            Assert.AreEqual(orderBook.TopAsks[2].Price, 3988.62);
            Assert.AreEqual(orderBook.TopAsks[2].Quantity, 8);

            Assert.AreEqual(orderBook.TopBids[0].Price, 3988.51);
            Assert.AreEqual(orderBook.TopBids[0].Quantity, 56);
            Assert.AreEqual(orderBook.TopBids[1].Price, 3988.50);
            Assert.AreEqual(orderBook.TopBids[1].Quantity, 44);
            Assert.AreEqual(orderBook.TopBids[2].Price, 3988.49);
            Assert.AreEqual(orderBook.TopBids[2].Quantity, 100);
            Assert.AreEqual(orderBook.TopBids[3].Price, 3988.48);
            Assert.AreEqual(orderBook.TopBids[3].Quantity, 10);
        }
    }
}
