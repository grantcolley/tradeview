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
            Assert.AreEqual(tradeCache.IncrementalSize, 5);
            Assert.AreEqual(tradeCache.CacheSize, incrementalSize);
            Assert.IsNull(tradeCache.GetLastTrade());
            Assert.IsNull(tradeCache.GetTrades());
            Assert.IsNull(tradeCache.GetLastTrades(1));
            Assert.IsNull(tradeCache.GetLastTrades(5));
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

        [TestMethod]
        public void TradeCache_AddTradeRange_NoResize()
        {
            // Arrange
            int incrementalSize = 5;
            TradeCache<Trade> tradeCache = new TradeCache<Trade>(incrementalSize);
            var trade1 = new Trade { Id = 1, Price = 0123m, Quantity = 1000m, Time = new DateTime(2000, 1, 1, 1, 0, 1) };
            var trade2 = new Trade { Id = 2, Price = 0121m, Quantity = 500m, Time = new DateTime(2000, 1, 1, 1, 0, 2) };
            var trade3 = new Trade { Id = 3, Price = 0124m, Quantity = 750m, Time = new DateTime(2000, 1, 1, 1, 0, 3) };

            var tradeRange = new[] { trade1, trade2, trade3 };

            // Act
            tradeCache.AddRange(tradeRange);

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

        [TestMethod]
        public void TradeCache_AddTrades_Resize()
        {
            // Arrange
            int incrementalSize = 5;
            TradeCache<Trade> tradeCache = new TradeCache<Trade>(incrementalSize);
            var trade1 = new Trade { Id = 1, Price = 0123m, Quantity = 1000m, Time = new DateTime(2000, 1, 1, 1, 0, 1) };
            var trade2 = new Trade { Id = 2, Price = 0121m, Quantity = 500m, Time = new DateTime(2000, 1, 1, 1, 0, 2) };
            var trade3 = new Trade { Id = 3, Price = 0124m, Quantity = 750m, Time = new DateTime(2000, 1, 1, 1, 0, 3) };
            var trade4 = new Trade { Id = 4, Price = 0122m, Quantity = 2000m, Time = new DateTime(2000, 1, 1, 1, 0, 4) };
            var trade5 = new Trade { Id = 5, Price = 0120m, Quantity = 1500m, Time = new DateTime(2000, 1, 1, 1, 0, 5) };
            var trade6 = new Trade { Id = 6, Price = 0119m, Quantity = 2750m, Time = new DateTime(2000, 1, 1, 1, 0, 6) };

            // Act
            tradeCache.Add(trade1);
            tradeCache.Add(trade2);
            tradeCache.Add(trade3);
            tradeCache.Add(trade4);
            tradeCache.Add(trade5);
            tradeCache.Add(trade6);

            var trades = tradeCache.GetTrades();
            var lastTrade = tradeCache.GetLastTrade();
            var lastTradesOver = tradeCache.GetLastTrades(10);
            var lastTradesUnder = tradeCache.GetLastTrades(2);

            // Assert
            Assert.AreEqual(tradeCache.Position, 5);
            Assert.AreEqual(tradeCache.CacheSize, 10);
            Assert.AreEqual(tradeCache.IncrementalSize, 5);
            Assert.AreEqual(trades.Length, 6);

            Assert.AreEqual(lastTrade, trade6);

            Assert.AreEqual(lastTradesOver.Length, 6);
            Assert.AreEqual(lastTradesOver[0], trade1);
            Assert.AreEqual(lastTradesOver[1], trade2);
            Assert.AreEqual(lastTradesOver[2], trade3);
            Assert.AreEqual(lastTradesOver[3], trade4);
            Assert.AreEqual(lastTradesOver[4], trade5);
            Assert.AreEqual(lastTradesOver[5], trade6);

            Assert.AreEqual(lastTradesUnder.Length, 2);
            Assert.AreEqual(lastTradesUnder[0], trade5);
            Assert.AreEqual(lastTradesUnder[1], trade6);
        }

        [TestMethod]
        public void TradeCache_AddTradeRange_Resize()
        {
            // Arrange
            int incrementalSize = 5;
            TradeCache<Trade> tradeCache = new TradeCache<Trade>(incrementalSize);
            var trade1 = new Trade { Id = 1, Price = 0123m, Quantity = 1000m, Time = new DateTime(2000, 1, 1, 1, 0, 1) };
            var trade2 = new Trade { Id = 2, Price = 0121m, Quantity = 500m, Time = new DateTime(2000, 1, 1, 1, 0, 2) };
            var trade3 = new Trade { Id = 3, Price = 0124m, Quantity = 750m, Time = new DateTime(2000, 1, 1, 1, 0, 3) };
            var trade4 = new Trade { Id = 4, Price = 0122m, Quantity = 2000m, Time = new DateTime(2000, 1, 1, 1, 0, 4) };
            var trade5 = new Trade { Id = 5, Price = 0120m, Quantity = 1500m, Time = new DateTime(2000, 1, 1, 1, 0, 5) };
            var trade6 = new Trade { Id = 6, Price = 0119m, Quantity = 2750m, Time = new DateTime(2000, 1, 1, 1, 0, 6) };

            var tradeRange = new[] { trade1, trade2, trade3, trade4, trade5, trade6 };

            // Act
            tradeCache.AddRange(tradeRange);

            var trades = tradeCache.GetTrades();
            var lastTrade = tradeCache.GetLastTrade();
            var lastTradesOver = tradeCache.GetLastTrades(10);
            var lastTradesUnder = tradeCache.GetLastTrades(2);

            // Assert
            Assert.AreEqual(tradeCache.Position, 5);
            Assert.AreEqual(tradeCache.CacheSize, 10);
            Assert.AreEqual(tradeCache.IncrementalSize, 5);
            Assert.AreEqual(trades.Length, 6);

            Assert.AreEqual(lastTrade, trade6);

            Assert.AreEqual(lastTradesOver.Length, 6);
            Assert.AreEqual(lastTradesOver[0], trade1);
            Assert.AreEqual(lastTradesOver[1], trade2);
            Assert.AreEqual(lastTradesOver[2], trade3);
            Assert.AreEqual(lastTradesOver[3], trade4);
            Assert.AreEqual(lastTradesOver[4], trade5);
            Assert.AreEqual(lastTradesOver[5], trade6);

            Assert.AreEqual(lastTradesUnder.Length, 2);
            Assert.AreEqual(lastTradesUnder[0], trade5);
            Assert.AreEqual(lastTradesUnder[1], trade6);
        }
    }
}
