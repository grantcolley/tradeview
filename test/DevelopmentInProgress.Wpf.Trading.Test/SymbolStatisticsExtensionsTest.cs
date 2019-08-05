using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DevelopmentInProgress.Wpf.Trading.Test
{
    [TestClass]
    public class SymbolStatisticsExtensionsTest
    {
        [TestMethod]
        public void GetViewSymbolStatistics_Pass()
        {
            // Arrange
            var interfaceStats = TestHelper.TrxStats;

            // Act
            var viewStats = interfaceStats.GetViewSymbolStatistics();

            // Assert
            Assert.AreEqual(viewStats.FirstTradeId, interfaceStats.FirstTradeId);
            Assert.AreEqual(viewStats.CloseTime, interfaceStats.CloseTime);
            Assert.AreEqual(viewStats.OpenTime, interfaceStats.OpenTime);
            Assert.AreEqual(viewStats.QuoteVolume, interfaceStats.QuoteVolume);
            Assert.AreEqual(viewStats.Volume, Convert.ToInt64(interfaceStats.Volume));
            Assert.AreEqual(viewStats.LowPrice, interfaceStats.LowPrice);
            Assert.AreEqual(viewStats.HighPrice, interfaceStats.HighPrice);
            Assert.AreEqual(viewStats.OpenPrice, interfaceStats.OpenPrice);
            Assert.AreEqual(viewStats.AskQuantity, interfaceStats.AskQuantity);
            Assert.AreEqual(viewStats.AskPrice, interfaceStats.AskPrice);
            Assert.AreEqual(viewStats.BidQuantity, interfaceStats.BidQuantity);
            Assert.AreEqual(viewStats.BidPrice, interfaceStats.BidPrice);
            Assert.AreEqual(viewStats.LastQuantity, interfaceStats.LastQuantity);
            Assert.AreEqual(viewStats.LastPrice, interfaceStats.LastPrice);
            Assert.AreEqual(viewStats.PreviousClosePrice, interfaceStats.PreviousClosePrice);
            Assert.AreEqual(viewStats.WeightedAveragePrice, interfaceStats.WeightedAveragePrice);
            Assert.AreEqual(viewStats.PriceChangePercent, decimal.Round(interfaceStats.PriceChangePercent, 2, MidpointRounding.AwayFromZero));
            Assert.AreEqual(viewStats.PriceChange, interfaceStats.PriceChange);
            Assert.AreEqual(viewStats.Period, interfaceStats.Period);
            Assert.AreEqual(viewStats.Symbol, interfaceStats.Symbol);
            Assert.AreEqual(viewStats.LastTradeId, interfaceStats.LastTradeId);
            Assert.AreEqual(viewStats.TradeCount, interfaceStats.TradeCount);
        }
    }
}
