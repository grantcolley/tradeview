using DevelopmentInProgress.MarketView.Interface.Extensions;
using DevelopmentInProgress.MarketView.Interface.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.MarketView.Interface.Test
{
    [TestClass]
    public class OrderExtensionsTest
    {
        [TestMethod]
        public void IsMarketOrder_GetOrderTypeName_Pass()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsTrue(OrderType.Market.GetOrderTypeName().IsMarketOrder());
            Assert.IsFalse(OrderType.Limit.GetOrderTypeName().IsMarketOrder());
            Assert.IsFalse(OrderType.LimitMaker.GetOrderTypeName().IsMarketOrder());
            Assert.IsTrue(OrderType.StopLoss.GetOrderTypeName().IsMarketOrder());
            Assert.IsTrue(OrderType.TakeProfit.GetOrderTypeName().IsMarketOrder());
            Assert.IsFalse(OrderType.StopLossLimit.GetOrderTypeName().IsMarketOrder());
            Assert.IsFalse(OrderType.TakeProfitLimit.GetOrderTypeName().IsMarketOrder());
        }

        [TestMethod]
        public void IsStopLoss_GetOrderTypeName_Pass()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsFalse(OrderType.Market.GetOrderTypeName().IsStopLoss());
            Assert.IsFalse(OrderType.Limit.GetOrderTypeName().IsStopLoss());
            Assert.IsFalse(OrderType.LimitMaker.GetOrderTypeName().IsStopLoss());
            Assert.IsTrue(OrderType.StopLoss.GetOrderTypeName().IsStopLoss());
            Assert.IsTrue(OrderType.TakeProfit.GetOrderTypeName().IsStopLoss());
            Assert.IsTrue(OrderType.StopLossLimit.GetOrderTypeName().IsStopLoss());
            Assert.IsTrue(OrderType.TakeProfitLimit.GetOrderTypeName().IsStopLoss());
        }
    }
}
