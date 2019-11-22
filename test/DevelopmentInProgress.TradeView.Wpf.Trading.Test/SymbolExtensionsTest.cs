using DevelopmentInProgress.TradeView.Test.Helper;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.Test
{
    [TestClass]
    public class SymbolExtensionsTest
    {
        [TestMethod]
        public void ConvertSymbols_Pass()
        {
            // Arrange
            var symbol = TestHelper.Trx;

            // Act
            var viewSymbol = symbol.GetViewSymbol();
            var interfaceSymbol = viewSymbol.GetInterfaceSymbol();
            var missingOrderTypes = symbol.OrderTypes.Except(interfaceSymbol.OrderTypes).ToList();
            missingOrderTypes.AddRange(interfaceSymbol.OrderTypes.Except(symbol.OrderTypes));

            // Assert
            Assert.AreEqual(symbol.NotionalMinimumValue, interfaceSymbol.NotionalMinimumValue);
            Assert.AreEqual(symbol.BaseAsset.Symbol, interfaceSymbol.BaseAsset.Symbol);
            Assert.AreEqual(symbol.BaseAsset.Precision, interfaceSymbol.BaseAsset.Precision);
            Assert.AreEqual(symbol.QuoteAsset.Symbol, interfaceSymbol.QuoteAsset.Symbol);
            Assert.AreEqual(symbol.QuoteAsset.Precision, interfaceSymbol.QuoteAsset.Precision);
            Assert.AreEqual(symbol.Price.Minimum, interfaceSymbol.Price.Minimum);
            Assert.AreEqual(symbol.Price.Maximum, interfaceSymbol.Price.Maximum);
            Assert.AreEqual(symbol.Price.Increment, interfaceSymbol.Price.Increment);
            Assert.AreEqual(symbol.Quantity.Minimum, interfaceSymbol.Quantity.Minimum);
            Assert.AreEqual(symbol.Quantity.Maximum, interfaceSymbol.Quantity.Maximum);
            Assert.AreEqual(symbol.Quantity.Increment, interfaceSymbol.Quantity.Increment);
            Assert.AreEqual(symbol.Status, interfaceSymbol.Status);
            Assert.AreEqual(symbol.IsIcebergAllowed, interfaceSymbol.IsIcebergAllowed);
            Assert.IsFalse(missingOrderTypes.Any());
        }

        [TestMethod]
        public void JoinSymbolStatistics()
        {
            // Arrange 
            var symbol = TestHelper.Eth.GetViewSymbol();
            var statistics = TestHelper.EthStats.GetViewSymbolStatistics();

            // Act
            var symbols = symbol.JoinStatistics(statistics);

            // Assert
            Assert.AreEqual(symbol.SymbolStatistics, statistics);
            Assert.AreEqual(symbol.PriceChangePercentDirection, statistics.PriceChangePercent > 0 ? 1 : statistics.PriceChangePercent < 0 ? -1 : 0);
        }

        [TestMethod]
        public void UpdateStatistics_PriceChange_Upwards()
        {
            // Arrange 
            var symbol = TestHelper.Eth.GetViewSymbol();
            var statistics = TestHelper.EthStats;
            symbol.JoinStatistics(statistics.GetViewSymbolStatistics());
            var updatedStatistics = TestHelper.EthStats_UpdatedLastPrice_Upwards;

            // Act
            var symbols = symbol.UpdateStatistics(updatedStatistics);

            // Assert
            Assert.AreEqual(symbol.SymbolStatistics.PriceChangePercent, updatedStatistics.PriceChangePercent);
            Assert.AreEqual(symbol.LastPriceChangeDirection, 1);
            Assert.AreEqual(symbol.SymbolStatistics.LastPrice, updatedStatistics.LastPrice);
        }

        [TestMethod]
        public void UpdateStatistics_PriceChange_Downwards()
        {
            // Arrange 
            var symbol = TestHelper.Eth.GetViewSymbol();
            var statistics = TestHelper.EthStats;
            symbol.JoinStatistics(statistics.GetViewSymbolStatistics());
            var updatedStatistics = TestHelper.EthStats_UpdatedLastPrice_Downwards;

            // Act
            var symbols = symbol.UpdateStatistics(updatedStatistics);

            // Assert
            Assert.AreEqual(symbol.SymbolStatistics.PriceChangePercent, updatedStatistics.PriceChangePercent);
            Assert.AreEqual(symbol.LastPriceChangeDirection, -1);
            Assert.AreEqual(symbol.SymbolStatistics.LastPrice, updatedStatistics.LastPrice);
        }
    }
}
