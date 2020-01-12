using DevelopmentInProgress.TradeView.Service;
using DevelopmentInProgress.TradeView.Wpf.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Test
{
    [TestClass]
    public class HelperFactoryContainerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var helperFactoryContainer = new HelperFactoryContainer(
                new TradeHelperFactory(new ExchangeApiFactory()),
                new OrderBookHelperFactory(new ExchangeApiFactory()));

            // Act
            var factory1 = helperFactoryContainer.GetFactory<ITradeHelperFactory>();
            var factory2 = helperFactoryContainer.GetFactory<IOrderBookHelperFactory>();

            // Assert
            Assert.IsInstanceOfType(factory1, typeof(TradeHelperFactory));
            Assert.IsInstanceOfType(factory2, typeof(OrderBookHelperFactory));
        }
    }
}
