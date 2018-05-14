using DevelopmentInProgress.MarketView.Interface.Extensions;
using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.Validation;
using DevelopmentInProgress.MarketView.Test.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.MarketView.Interface.Test
{
    [TestClass]
    public class SymbolsExtensionsTest
    {
        private static Symbol trx;
        private static SymbolStats trxStats;
        private static Symbol eth;
        private static SymbolStats ethStats;

        [ClassInitialize()]
        public static void SymbolsExtensionsTest_Initialize(TestContext testContext)
        {
            eth = TestHelper.Eth;
            ethStats = TestHelper.EthStats;

            trx = TestHelper.Trx;
            trxStats = TestHelper.TrxStats;
        }

        [TestMethod]
        [ExpectedException(typeof(OrderValidationException))]
        public void Limit_Failed_NoPrice()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Type = OrderType.Limit, Quantity = 500.00000000M };

            // Act
            trx.ValidateClientOrder(clientOrder);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Limit_Pass()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Type = OrderType.Limit, Quantity = 500.00000000M, Price = trxStats.BidPrice };

            // Act
            trx.ValidateClientOrder(clientOrder);

            // Assert
            // Expected no exception
        }

        [TestMethod]
        [ExpectedException(typeof(OrderValidationException))]
        public void LimitMaker_Failed_NoPrice()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Type = OrderType.LimitMaker, Quantity = 500.00000000M };

            // Act
            trx.ValidateClientOrder(clientOrder);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void LimitMaker_Pass()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Type = OrderType.LimitMaker, Quantity = 500.00000000M, Price = trxStats.BidPrice };

            // Act
            trx.ValidateClientOrder(clientOrder);

            // Assert
            // Expected no exception
        }

        [TestMethod]
        [ExpectedException(typeof(OrderValidationException))]
        public void StopLossLimit_Failed_NoPrice_NoStopPrice()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Type = OrderType.StopLossLimit, Quantity = 500.00000000M };

            // Act
            try
            {
                eth.ValidateClientOrder(clientOrder);
            }
            catch (OrderValidationException e)
            {
                // Assert
                Assert.IsTrue(e.Message.Equals($"ETHBTC {clientOrder.Type.GetOrderTypeName()} order not valid: Notional 0 is less than the minimum notional {eth.NotionalMinimumValue};Price 0 cannot be below the minimum {eth.Price.Minimum};Stop Price 0 cannot be below the minimum {eth.Price.Minimum}"));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OrderValidationException))]
        public void StopLossLimit_Failed_NoStopPrice()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Type = OrderType.StopLossLimit, Quantity = 500.00000000M, Price = ethStats.BidPrice };

            // Act
            try
            {
                eth.ValidateClientOrder(clientOrder);
            }
            catch (OrderValidationException e)
            {
                // Assert
                Assert.IsTrue(e.Message.Equals($"ETHBTC {clientOrder.Type.GetOrderTypeName()} order not valid: Stop Price 0 cannot be below the minimum {eth.Price.Minimum}"));
                throw;
            }
        }

        [TestMethod]
        public void StopLossLimit_Pass()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Type = OrderType.StopLossLimit, Quantity = 500.00000000M, Price = ethStats.BidPrice, StopPrice = ethStats.BidPrice };

            // Act
            eth.ValidateClientOrder(clientOrder);

            // Assert
            // Expected no exception
        }

        [TestMethod]
        [ExpectedException(typeof(OrderValidationException))]
        public void TakeProfitLimit_Failed_NoPrice_NoStopPrice()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Type = OrderType.TakeProfitLimit, Quantity = 500.00000000M };

            // Act
            try
            {
                eth.ValidateClientOrder(clientOrder);
            }
            catch (OrderValidationException e)
            {
                // Assert
                Assert.IsTrue(e.Message.Equals($"ETHBTC {clientOrder.Type.GetOrderTypeName()} order not valid: Notional 0 is less than the minimum notional {eth.NotionalMinimumValue};Price 0 cannot be below the minimum {eth.Price.Minimum};Stop Price 0 cannot be below the minimum {eth.Price.Minimum}"));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OrderValidationException))]
        public void TakeProfitLimit_Failed_NoStopPrice()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Type = OrderType.TakeProfitLimit, Quantity = 500.00000000M, Price = ethStats.BidPrice };

            // Act
            try
            {
                eth.ValidateClientOrder(clientOrder);
            }
            catch (OrderValidationException e)
            {
                // Assert
                Assert.IsTrue(e.Message.Equals($"ETHBTC {clientOrder.Type.GetOrderTypeName()} order not valid: Stop Price 0 cannot be below the minimum {eth.Price.Minimum}"));
                throw;
            }
        }

        [TestMethod]
        public void TakeProfitLimit_Pass()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Type = OrderType.TakeProfitLimit, Quantity = 500.00000000M, Price = ethStats.BidPrice, StopPrice = ethStats.BidPrice };

            // Act
            eth.ValidateClientOrder(clientOrder);

            // Assert
            // Expected no exception
        }

        [TestMethod]
        [ExpectedException(typeof(OrderValidationException))]
        public void Market_Fail__NoQuantity()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Type = OrderType.Market, Price = ethStats.LastPrice };

            // Act
            try
            {
                eth.ValidateClientOrder(clientOrder);
            }
            catch (OrderValidationException e)
            {
                // Assert
                Assert.IsTrue(e.Message.Equals($"ETHBTC {clientOrder.Type.GetOrderTypeName()} order not valid: Quantity 0 is below the minimum {eth.Quantity.Minimum};Notional {clientOrder.Price * clientOrder.Quantity} is less than the minimum notional {eth.NotionalMinimumValue}"));
                throw;
            }
        }

        [TestMethod]
        public void Market_Pass()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Type = OrderType.Market, Quantity = 500, Price = ethStats.LastPrice };

            // Act
            eth.ValidateClientOrder(clientOrder);

            // Assert
            // Expected no exception
        }
    }
}
