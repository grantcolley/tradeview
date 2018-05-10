using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Test.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Linq;
using DevelopmentInProgress.Wpf.MarketView.Extensions;
using System;

namespace DevelopmentInProgress.Wpf.MarketView.Test
{
    [TestClass]
    public class OrderExtensionsTest
    {
        [TestMethod]
        public void UpdateOrder_Pass()
        {
            // Arrange
            var originalOrder = MarketHelper.Orders.First();
            var serialisedOrder = JsonConvert.SerializeObject(originalOrder);
            var interfaceOrder = JsonConvert.DeserializeObject<Interface.Order>(serialisedOrder);
            var updatedInterfaceOrder = JsonConvert.DeserializeObject<Interface.Order>(serialisedOrder);

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
            var originalOrder = MarketHelper.Orders.First();
            var serialisedOrder = JsonConvert.SerializeObject(originalOrder);
            var interfaceOrder = JsonConvert.DeserializeObject<Interface.Order>(serialisedOrder);
            var updatedInterfaceOrder = JsonConvert.DeserializeObject<Interface.Order>(serialisedOrder);

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
