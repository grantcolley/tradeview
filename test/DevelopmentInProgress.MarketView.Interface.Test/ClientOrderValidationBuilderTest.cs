using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.Validation;
using DevelopmentInProgress.MarketView.Test.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DevelopmentInProgress.MarketView.Interface.Test
{
    [TestClass]
    public class ClientOrderValidationBuilderTest
    {
        [TestMethod]
        public void BaseValidation_Fail_NoSymbol_OrderType_MinQty_StepSize()
        {
            // Arrange
            string message;
            var trx = TestHelper.Trx;
            var trxStats = TestHelper.TrxStats;
            var clientOrder = new ClientOrder() { Type = OrderType.Limit, Quantity = 0.00090000M, Price = trxStats.LastPrice };

            trx.OrderTypes = trx.OrderTypes.Where(t => t != OrderType.Limit);

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder().Build();
            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(message, $" {clientOrder.Type} order not valid: Order has no symbol;Limit order is not permitted;Quantity {clientOrder.Quantity} is below the minimum {trx.Quantity.Minimum};Quantity {clientOrder.Quantity} must be in multiples of the step size {trx.Quantity.Increment};Notional {clientOrder.Price * clientOrder.Quantity} is less than the minimum notional {trx.NotionalMinimumValue}");
        }

        [TestMethod]
        public void BaseValidation_Fail_NoSymbol_OrderType_MaxQty()
        {
            // Arrange
            string message;
            var trx = TestHelper.Trx;
            var trxStats = TestHelper.TrxStats;
            var clientOrder = new ClientOrder() { Type = OrderType.Limit, Quantity = 150000000.00000000M, Price = trxStats.LastPrice };

            trx.OrderTypes = trx.OrderTypes.Where(t => t != OrderType.Limit);

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder().Build();
            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(message, $" {clientOrder.Type} order not valid: Order has no symbol;Limit order is not permitted;Quantity {clientOrder.Quantity} is above the maximum {trx.Quantity.Maximum}");
        }

        [TestMethod]
        public void BaseValidation_Failed_SymbolMismatch()
        {
            // Arrange
            string message;
            var trx = TestHelper.Trx;
            var trxStats = TestHelper.TrxStats;
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Type = OrderType.Limit, Quantity = 500.00000000M, Price = trxStats.LastPrice };

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder().Build();
            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(message, $"{clientOrder.Symbol} {clientOrder.Type} order not valid: Order {clientOrder.Symbol} validation symbol {trx.BaseAsset.Symbol}{trx.QuoteAsset.Symbol} mismatch");
        }

        [TestMethod]
        public void BaseValidation_Pass()
        {
            // Arrange
            string message;
            var trx = TestHelper.Trx;
            var trxStats = TestHelper.TrxStats;
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Type = OrderType.Limit, Quantity = 500.00000000M, Price = trxStats.LastPrice };

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder().Build();
            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(message, string.Empty);
        }

        [TestMethod]
        public void PriceValidation_Failed_MinPrice_TickSize()
        {
            // Arrange
            string message;
            var trx = TestHelper.Trx;
            var trxStats = TestHelper.TrxStats;
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Type = OrderType.Limit, Quantity = 500.00000000M, Price = 0.000000001M };
            
            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder()
                .AddPriceValidation()
                .Build();

            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(message, $"{clientOrder.Symbol} {clientOrder.Type} order not valid: Notional {clientOrder.Price * clientOrder.Quantity} is less than the minimum notional {trx.NotionalMinimumValue};Price {clientOrder.Price} cannot be below the minimum {trx.Price.Minimum};Price {clientOrder.Price} doesn't meet the tick size {trx.Price.Increment}");
        }

        [TestMethod]
        public void PriceValidation_Failed_MaxPrice()
        {
            // Arrange
            string message;
            var trx = TestHelper.Trx;
            var trxStats = TestHelper.TrxStats;
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Type = OrderType.Limit, Quantity = 500.00000000M, Price = 15000000.00000000M };

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder()
                .AddPriceValidation()
                .Build();

            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(message, $"{clientOrder.Symbol} {clientOrder.Type} order not valid: Price {clientOrder.Price} cannot be above the maximum {trx.Price.Maximum}");
        }

        [TestMethod]
        public void PriceValidation_Pass()
        {
            // Arrange
            string message;
            var trx = TestHelper.Trx;
            var trxStats = TestHelper.TrxStats;
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Type = OrderType.Limit, Quantity = 500.00000000M, Price = trxStats.LastPrice };

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder()
                .AddPriceValidation()
                .Build();

            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(message, string.Empty);
        }

        [TestMethod]
        public void StopPriceValidation_Failed_MinPrice_TickSize()
        {
            // Arrange
            string message;
            var trx = TestHelper.Trx;
            var trxStats = TestHelper.TrxStats;
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Type = OrderType.Limit, Quantity = 500.00000000M, Price = trxStats.LastPrice, StopPrice = 0.000000001M };

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder()
                .AddPriceValidation()
                .AddStopPriceValidation()
                .Build();

            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(message, $"{clientOrder.Symbol} {clientOrder.Type} order not valid: Stop Price {clientOrder.StopPrice} cannot be below the minimum {trx.Price.Minimum};Stop Price {clientOrder.StopPrice} doesn't meet the tick size {trx.Price.Increment}");
        }

        [TestMethod]
        public void StopPriceValidation_Failed_MaxPrice()
        {
            // Arrange
            string message;
            var trx = TestHelper.Trx;
            var trxStats = TestHelper.TrxStats;
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Type = OrderType.Limit, Quantity = 500.00000000M, Price = trxStats.LastPrice, StopPrice = 15000000.00000000M };

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder()
                .AddPriceValidation()
                .AddStopPriceValidation()
                .Build();

            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(message, $"{clientOrder.Symbol} {clientOrder.Type} order not valid: Stop Price {clientOrder.StopPrice} cannot be above the maximum {trx.Price.Maximum}");
        }

        [TestMethod]
        public void StopPriceValidation_Pass()
        {
            // Arrange
            string message;
            var trx = TestHelper.Trx;
            var trxStats = TestHelper.TrxStats;
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Type = OrderType.Limit, Quantity = 500.00000000M, Price = trxStats.LastPrice, StopPrice = (trxStats.LastPrice + (100 * trx.Price.Increment)) };

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder()
                .AddPriceValidation()
                .AddStopPriceValidation()
                .Build();

            var result = clientOrderValidation.TryValidate(trx, clientOrder, out message);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(message, string.Empty);
        }
    }
}
