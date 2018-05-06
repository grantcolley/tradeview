using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentInProgress.MarketView.Interface.Test
{
    [TestClass]
    public class ClientOrderValidationBuilderTest
    {
        private static List<Symbol> symbols;
        private static Symbol trx;

        [ClassInitialize()]
        public static void ClientOrderValidationBuilderTest_Initializ(TestContext testContext)
        {
            var marketHelper = new MarketHelper();

            symbols = marketHelper.Symbols;

            trx = symbols.Single(s => s.BaseAsset.Symbol.Equals("TRX") && s.QuoteAsset.Symbol.Equals("BTC"));
        }

        [TestMethod]
        public void BaseValidation_FailWithNoSymbolAndBelowMinQty()
        {
            // Arrange
            string message;
            var clientOrder = new ClientOrder() { Quantity = 0.00090000M };

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder().Build();
            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);
            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(message, " Limit order not valid: Order has no symbol;Limit order is not permitted;Quantity 0.00090000 is below the minimum 0.00100000;Quantity 0.00090000 must be in multiples of the step size 0.001");
        }

        [TestMethod]
        public void BaseValidation_FailWithNoSymbolAndAboveMaxQty()
        {
            // Arrange
            string message;
            var clientOrder = new ClientOrder() { Quantity = 100001.00000000M };

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder().Build();
            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(message, " Limit order not valid: Order has no symbol;Limit order is not permitted;Quantity 100001.00000000 is above the maximum 100000.00000000");
        }

        [TestMethod]
        public void BaseValidation_FailWithSymbolMismatch()
        {
            // Arrange
            string message;
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Quantity = 100001.00000000M };

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder().Build();
            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(message, "ETHBTC Limit order not valid: Order ETHBTC validation symbol TRXBTC mismatch;Limit order is not permitted;Quantity 100001.00000000 is above the maximum 100000.00000000");
        }

        [TestMethod]
        public void BaseValidation_Pass()
        {
            // Arrange
            string message;
            var clientOrder = new ClientOrder() { Quantity = 0.00000000M };

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder().Build();
            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(message, " Limit order not valid: No symbol;Limit order is not permitted;Quantity 100001.00000000 is above the maximum 100000.00000000");
        }
    }
}
