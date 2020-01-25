using System;
using DevelopmentInProgress.Strategy.Common.Parameter;
using DevelopmentInProgress.Strategy.Common.TradeCreator;
using DevelopmentInProgress.TradeView.Interface.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.Strategy.Common.Test
{
    [TestClass]
    public class MovingAverageTradeCreatorTest
    {
        [TestMethod]
        public void MovingAverageTradeCreator_Initialise()
        {
            // Arrange
            MovingAverageTradeCreator movingAverageTradeCreator;

            // Act
            movingAverageTradeCreator = new MovingAverageTradeCreator();

            // Assert
            Assert.AreEqual(movingAverageTradeCreator.GetCurrentPeriod(), 0);
            Assert.AreEqual(movingAverageTradeCreator.GetMovingAvarageRange(), 0);
            Assert.AreEqual(movingAverageTradeCreator.GetBuyIndicator(), 0);
            Assert.AreEqual(movingAverageTradeCreator.GetSellIndicator(), 0);

            var range = movingAverageTradeCreator.GetRange();
            Assert.AreEqual(range.Length, 0);
        }

        [TestMethod]
        public void MovingAverageTradeCreator_CreateTrades()
        {
            // Arrange
            var movingAverageTradeCreator = new MovingAverageTradeCreator();
            var movingAverageTradeParameters = new MovingAverageTradeParameters
            {
                MovingAvarageRange = 5,
                BuyIndicator = 123.45m,
                SellIndicator = 125.67m
            };

            movingAverageTradeCreator.Reset(movingAverageTradeParameters);

            // Act
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 0 });
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 1 });
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 2 });
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 3 });
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 4 });

            // Assert
            Assert.AreEqual(movingAverageTradeCreator.GetCurrentPeriod(), 4);
            Assert.AreEqual(movingAverageTradeCreator.GetMovingAvarageRange(), movingAverageTradeParameters.MovingAvarageRange);
            Assert.AreEqual(movingAverageTradeCreator.GetBuyIndicator(), movingAverageTradeParameters.BuyIndicator);
            Assert.AreEqual(movingAverageTradeCreator.GetSellIndicator(), movingAverageTradeParameters.SellIndicator);

            var range = movingAverageTradeCreator.GetRange();
            Assert.AreEqual(range.Length, movingAverageTradeParameters.MovingAvarageRange);

            for(int i = 0; i < movingAverageTradeParameters.MovingAvarageRange; i++)
            {
                range[i] = i;
            }
        }
    }
}
