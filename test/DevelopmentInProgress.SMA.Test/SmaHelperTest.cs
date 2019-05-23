using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.SMA.Test
{
    [TestClass]
    public class SmaHelperTest
    {
        [TestMethod]
        public void CalculateMovingVolatility()
        {
            // Arrange
            var deviationsSquared = new double[] { 0, 4, 25, 4, 9, 25, 0, 1, 16, 4, 16, 0, 9, 25, 4, 9, 9, 4, 1, 4, 9 };

            // Act
            var volatility = SmaHelper.CalculateMovingVolatility(20, deviationsSquared, 20);

            // Assert
            Assert.AreEqual(Math.Round(volatility, 3), 3.061);
        }
    }
}
