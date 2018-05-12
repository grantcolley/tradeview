using DevelopmentInProgress.MarketView.Interface.Extensions;
using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Test.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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

        [TestMethod]
        public void GetOrderTypeNames_Pass()
        {
            // Arrange
            var trx = MarketHelper.Trx;
            var eth = MarketHelper.Eth;

            // Act
            var trxOrderTypes = trx.OrderTypes.GetOrderTypeNames();
            var ethOrderTypes = eth.OrderTypes.GetOrderTypeNames();

            // Assert
            Assert.AreEqual(trxOrderTypes.Length, trx.OrderTypes.Count());
            foreach(var orderType in trx.OrderTypes)
            {
                Assert.IsTrue(trxOrderTypes.Contains(orderType.GetOrderTypeName()));
            }

            Assert.AreEqual(ethOrderTypes.Length, eth.OrderTypes.Count());
            foreach (var orderType in eth.OrderTypes)
            {
                Assert.IsTrue(ethOrderTypes.Contains(orderType.GetOrderTypeName()));
            }
        }
    }
}
