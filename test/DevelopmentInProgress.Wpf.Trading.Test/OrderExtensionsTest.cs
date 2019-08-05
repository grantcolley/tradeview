using DevelopmentInProgress.MarketView.Test.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using DevelopmentInProgress.Wpf.Common.Extensions;
using System;

namespace DevelopmentInProgress.Wpf.Trading.Test
{
    [TestClass]
    public class OrderExtensionsTest
    {
        [TestMethod]
        public void UpdateOrder_Pass()
        {
            // Arrange
            var interfaceOrder = TestHelper.Orders.First();

            var updatedInterfaceOrder = TestHelper.Orders.First();
            updatedInterfaceOrder.ExecutedQuantity = 100;

            var order = interfaceOrder.GetViewOrder();
            var updatedOrder = updatedInterfaceOrder.GetViewOrder();

            // Act
            order.Update(updatedOrder);

            // Assert
            Assert.AreEqual(order.ExecutedQuantity, updatedOrder.ExecutedQuantity);
            Assert.AreEqual(order.FilledPercent, updatedOrder.FilledPercent);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void UpdateOrder_Fails_ClientOrderId()
        {
            // Arrange
            var interfaceOrder = TestHelper.Orders.First();
            var updatedInterfaceOrder = TestHelper.Orders.First();
            
            updatedInterfaceOrder.ClientOrderId = "123";

            var order = interfaceOrder.GetViewOrder();
            var updatedOrder = updatedInterfaceOrder.GetViewOrder();

            // Act
            try
            {
                order.Update(updatedOrder);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual(e.Message, $"{order.Symbol} order update failed: Cannot update ClientOrderId {order.ClientOrderId} with {updatedOrder.ClientOrderId}");                
                throw;
            }
        }
    }
}
