using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Test
{
    [TestClass]
    public class ClientOrderValidationBuilderTest
    {
        [TestMethod]
        public void BaseValidationFailed()
        {
            // Arrange
            string message;
            var symbol = new Symbol
            {
                OrderTypes = new List<OrderType>(),
                Price = new InclusiveRange { Minimum = 0.00000100M, Maximum = 100000.00000000M, Increment = 0.00000100M },
                Quantity = new InclusiveRange { Minimum = 0.00100000M, Maximum = 100000.00000000M, Increment = 0.00100000M }
            };

            var clientOrder = new ClientOrder();

            // Act
            var clientOrderValidation = new ClientOrderValidationBuilder()
                .Build();

            var result = clientOrderValidation.TryValidate(symbol, clientOrder, out message);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(message, "");
        }
    }
}
