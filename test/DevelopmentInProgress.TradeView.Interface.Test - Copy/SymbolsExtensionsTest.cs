using DevelopmentInProgress.TradeView.Core.Extensions;
using DevelopmentInProgress.TradeView.Core.Model;
using DevelopmentInProgress.TradeView.Core.Validation;
using DevelopmentInProgress.TradeView.Test.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.TradeView.Core.Test
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
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Side = OrderSide.Buy, Type = OrderType.Limit, Quantity = 500.00000000M };
            clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * clientOrder.Quantity };

            // Act
            trx.ValidateClientOrder(clientOrder);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Limit_Pass()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Side = OrderSide.Buy, Type = OrderType.Limit, Quantity = 500.00000000M, Price = trxStats.BidPrice };
            clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * clientOrder.Quantity };

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
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Side = OrderSide.Buy, Type = OrderType.LimitMaker, Quantity = 500.00000000M };
            clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * clientOrder.Quantity };

            // Act
            trx.ValidateClientOrder(clientOrder);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void LimitMaker_Pass()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Side = OrderSide.Buy, Type = OrderType.LimitMaker, Quantity = 500.00000000M, Price = trxStats.BidPrice };
            clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * clientOrder.Quantity };

            // Act
            trx.ValidateClientOrder(clientOrder);

            // Assert
            // Expected no exception
        }

        //[TestMethod]
        //[ExpectedException(typeof(OrderValidationException))]
        //public void StopLossLimit_Failed_NoPrice_NoStopPrice()
        //{
        //    // Arrange
        //    var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Side = OrderSide.Buy, Type = OrderType.StopLossLimit, Quantity = 500.00000000M };
        //    clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * clientOrder.Quantity };

        //    // Act
        //    try
        //    {
        //        eth.ValidateClientOrder(clientOrder);
        //    }
        //    catch (OrderValidationException e)
        //    {
        //        // Assert
        //        Assert.IsTrue(e.Message.Equals($"ETHBTC {clientOrder.Type.GetOrderTypeName()} order not valid: Notional 0 is less than the minimum notional {eth.NotionalMinimumValue}."));
        //        throw;
        //    }
        //}

        //[TestMethod]
        //[ExpectedException(typeof(OrderValidationException))]
        //public void StopLossLimit_Failed_NoStopPrice()
        //{
        //    // Arrange
        //    var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Side = OrderSide.Buy, Type = OrderType.StopLossLimit, Quantity = 500.00000000M, Price = ethStats.BidPrice };
        //    clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * clientOrder.Quantity };

        //    // Act
        //    try
        //    {
        //        eth.ValidateClientOrder(clientOrder);
        //    }
        //    catch (OrderValidationException e)
        //    {
        //        // Assert
        //        Assert.IsTrue(e.Message.Equals($"ETHBTC {clientOrder.Type.GetOrderTypeName()} order not valid: Stop Price 0 cannot be below the minimum {eth.Price.Minimum}"));
        //        throw;
        //    }
        //}

        [TestMethod]
        public void StopLossLimit_Pass()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Side = OrderSide.Buy, Type = OrderType.StopLossLimit, Quantity = 500.00000000M, Price = ethStats.BidPrice, StopPrice = ethStats.BidPrice };
            clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * clientOrder.Quantity };

            // Act
            eth.ValidateClientOrder(clientOrder);

            // Assert
            // Expected no exception
        }

        //[TestMethod]
        //[ExpectedException(typeof(OrderValidationException))]
        //public void TakeProfitLimit_Failed_NoPrice_NoStopPrice()
        //{
        //    // Arrange
        //    var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Side = OrderSide.Buy, Type = OrderType.TakeProfitLimit, Quantity = 500.00000000M };
        //    clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * clientOrder.Quantity };

        //    // Act
        //    try
        //    {
        //        eth.ValidateClientOrder(clientOrder);
        //    }
        //    catch (OrderValidationException e)
        //    {
        //        // Assert
        //        Assert.IsTrue(e.Message.Equals($"ETHBTC {clientOrder.Type.GetOrderTypeName()} order not valid: Notional 0 is less than the minimum notional {eth.NotionalMinimumValue};Price 0 cannot be below the minimum {eth.Price.Minimum};Stop Price 0 cannot be below the minimum {eth.Price.Minimum}"));
        //        throw;
        //    }
        //}

        //[TestMethod]
        //[ExpectedException(typeof(OrderValidationException))]
        //public void TakeProfitLimit_Failed_NoStopPrice()
        //{
        //    // Arrange
        //    var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Side = OrderSide.Buy, Type = OrderType.TakeProfitLimit, Quantity = 500.00000000M, Price = ethStats.BidPrice };
        //    clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * clientOrder.Quantity };

        //    // Act
        //    try
        //    {
        //        eth.ValidateClientOrder(clientOrder);
        //    }
        //    catch (OrderValidationException e)
        //    {
        //        // Assert
        //        Assert.IsTrue(e.Message.Equals($"ETHBTC {clientOrder.Type.GetOrderTypeName()} order not valid: Stop Price 0 cannot be below the minimum {eth.Price.Minimum}"));
        //        throw;
        //    }
        //}

        [TestMethod]
        public void TakeProfitLimit_Pass()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Side = OrderSide.Buy, Type = OrderType.TakeProfitLimit, Quantity = 500.00000000M, Price = ethStats.BidPrice, StopPrice = ethStats.BidPrice };
            clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * clientOrder.Quantity };

            // Act
            eth.ValidateClientOrder(clientOrder);

            // Assert
            // Expected no exception
        }

        [TestMethod]
        [ExpectedException(typeof(OrderValidationException))]
        public void Market_Fail_NoQuantity()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Side = OrderSide.Buy, Type = OrderType.Market, Price = ethStats.LastPrice };
            clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * clientOrder.Quantity };

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
            clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * clientOrder.Quantity };

            // Act
            eth.ValidateClientOrder(clientOrder);

            // Assert
            // Expected no exception
        }

        [TestMethod]
        [ExpectedException(typeof(OrderValidationException))]
        public void Market_Fail_AvailableFunds()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "ETHBTC", Side = OrderSide.Buy, Type = OrderType.Market, Quantity = 500, Price = ethStats.LastPrice };
            clientOrder.QuoteAccountBalance = new AccountBalance { Free = clientOrder.Price * 450M};

            // Act
            try
            {
                eth.ValidateClientOrder(clientOrder);
            }
            catch (OrderValidationException e)
            {
                // Assert
                Assert.IsTrue(e.Message.Equals($"ETHBTC {clientOrder.Type.GetOrderTypeName()} order not valid: Insufficient funds to buy: Indicative cost {clientOrder.Price * clientOrder.Quantity} is greater than the available funds {clientOrder.QuoteAccountBalance.Free}"));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OrderValidationException))]
        public void Buy_Fail_InsufficientFunds()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Side = OrderSide.Buy, Type = OrderType.Limit, Quantity = 500.00000000M, Price = trxStats.BidPrice };
            clientOrder.QuoteAccountBalance = new AccountBalance { Free = 0.00040000M};

            // Act
            try
            {
                trx.ValidateClientOrder(clientOrder);
            }
            catch (OrderValidationException e)
            {
                // Assert
                Assert.IsTrue(e.Message.Equals($"TRXBTC {clientOrder.Type.GetOrderTypeName()} order not valid: Insufficient funds to buy: {clientOrder.Price * clientOrder.Quantity} is greater than the available funds {clientOrder.QuoteAccountBalance.Free}"));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OrderValidationException))]
        public void Sell_Fail_InsufficientQuantity()
        {
            // Arrange
            var clientOrder = new ClientOrder() { Symbol = "TRXBTC", Side = OrderSide.Sell, Type = OrderType.Limit, Quantity = 500.00000000M, Price = trxStats.BidPrice };
            clientOrder.BaseAccountBalance = new AccountBalance { Free = 100 };

            // Act
            try
            {
                trx.ValidateClientOrder(clientOrder);
            }
            catch (OrderValidationException e)
            {
                // Assert
                Assert.IsTrue(e.Message.Equals($"TRXBTC {clientOrder.Type.GetOrderTypeName()} order not valid: Insufficient quantity to sell: {clientOrder.Quantity} is greater than the available quantity {clientOrder.BaseAccountBalance.Free}"));
                throw;
            }
        }
    }
}
