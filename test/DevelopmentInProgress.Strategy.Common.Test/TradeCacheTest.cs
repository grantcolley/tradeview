using DevelopmentInProgress.TradeView.Interface.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
            Assert.AreEqual(tradeCache.Position, -1);
            Assert.AreEqual(tradeCache.CacheSize, 0);
            Assert.AreEqual(tradeCache.IncrementalSize, 5);
            Assert.IsNull(tradeCache.GetLastTrade());
            Assert.AreEqual(tradeCache.GetTrades().Length, 0);
            Assert.AreEqual(tradeCache.GetLastTrades(1).Length, 0);
            Assert.AreEqual(tradeCache.GetLastTrades(5).Length, 0);
        }

        [TestMethod]
        public void TradeCache_AddTrades_NoResize()
        {
            // Arrange
            int incrementalSize = 5;
            TradeCache<Trade> tradeCache = new TradeCache<Trade>(incrementalSize);
            var trade1 = new Trade { Id = 1, Price = 0123m, Quantity = 1000m, Time = new DateTime(2000, 1, 1, 1, 0, 1)};
            var trade2 = new Trade { Id = 2, Price = 0121m, Quantity = 500m, Time = new DateTime(2000, 1, 1, 1, 0, 2) };
            var trade3 = new Trade { Id = 3, Price = 0124m, Quantity = 750m, Time = new DateTime(2000, 1, 1, 1, 0, 3) };

            // Act
            tradeCache.Add(trade1);
            tradeCache.Add(trade2);
            tradeCache.Add(trade3);

            var trades = tradeCache.GetTrades();
            var lastTrade = tradeCache.GetLastTrade();
            var lastTradesOver = tradeCache.GetLastTrades(4);
            var lastTradesUnder = tradeCache.GetLastTrades(2);

            // Assert
            Assert.AreEqual(tradeCache.Position, 2);
            Assert.AreEqual(tradeCache.CacheSize, 5);
            Assert.AreEqual(tradeCache.IncrementalSize, 5);
            Assert.AreEqual(trades.Length, 3);

            Assert.AreEqual(lastTrade, trade3);

            Assert.AreEqual(lastTradesOver.Length, 3);
            Assert.AreEqual(lastTradesOver[0], trade1);
            Assert.AreEqual(lastTradesOver[1], trade2);
            Assert.AreEqual(lastTradesOver[2], trade3);

            Assert.AreEqual(lastTradesUnder.Length, 2);
            Assert.AreEqual(lastTradesUnder[0], trade2);
            Assert.AreEqual(lastTradesUnder[1], trade3);
        }
    }
}
