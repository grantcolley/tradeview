using DevelopmentInProgress.TradeView.Interface.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.Strategy.Common.Test
{
    [TestClass]
    public class TradeCacheTest
    {
        [TestMethod]
        public void TradeCache_Initialise()
        {
            // Arrange
            int incrementalSize = 5;
            TradeCache<Trade> tradeCache;

            // Act
            tradeCache = new TradeCache<Trade>(incrementalSize);

            // Assert
            Assert.AreEqual(tradeCache.Position, 0);
            Assert.AreEqual(tradeCache.CacheSize, 0);
            Assert.AreEqual(tradeCache.IncrementalSize, 5);
            Assert.IsNull(tradeCache.GetLastTrade());
            Assert.AreEqual(tradeCache.GetTrades().Length, 0);
            Assert.AreEqual(tradeCache.GetLastTrades(1).Length, 0);
            Assert.AreEqual(tradeCache.GetLastTrades(5).Length, 0);
        }
    }
}
