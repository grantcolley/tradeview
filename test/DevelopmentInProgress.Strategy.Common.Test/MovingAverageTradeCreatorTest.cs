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
            Assert.AreEqual(movingAverageTradeCreator.GetCurrentPosition(), -1);
            Assert.AreEqual(movingAverageTradeCreator.GetMovingAvarageRange(), 0);
            Assert.AreEqual(movingAverageTradeCreator.GetBuyIndicator(), 0);
            Assert.AreEqual(movingAverageTradeCreator.GetSellIndicator(), 0);

            var range = movingAverageTradeCreator.GetRange();
            Assert.AreEqual(range.Length, 0);
        }

        [TestMethod]
        public void MovingAverageTradeCreator_Reset_CreateTrades_LessThanRange()
        {
            // Arrange
            var movingAverageTradeCreator = new MovingAverageTradeCreator();
            var movingAverageTradeParameters = new MovingAverageTradeParameters
            {
                MovingAvarageRange = 5,
                BuyIndicator = 123.45m,
                SellIndicator = 125.67m
            };

            // Act
            movingAverageTradeCreator.Reset(movingAverageTradeParameters);
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 1 });
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 2 });
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 3 });

            // Assert
            Assert.AreEqual(movingAverageTradeCreator.GetCurrentPosition(), 2);
            Assert.AreEqual(movingAverageTradeCreator.GetMovingAvarageRange(), movingAverageTradeParameters.MovingAvarageRange);
            Assert.AreEqual(movingAverageTradeCreator.GetBuyIndicator(), movingAverageTradeParameters.BuyIndicator);
            Assert.AreEqual(movingAverageTradeCreator.GetSellIndicator(), movingAverageTradeParameters.SellIndicator);

            var range = movingAverageTradeCreator.GetRange();
            Assert.AreEqual(range.Length, movingAverageTradeParameters.MovingAvarageRange);
            Assert.AreEqual(range[0], 1);
            Assert.AreEqual(range[1], 2);
            Assert.AreEqual(range[2], 3);
            Assert.AreEqual(range[3], 0);
            Assert.AreEqual(range[4], 0);
        }

        [TestMethod]
        public void MovingAverageTradeCreator_Reset_CreateTrades_EqualsRange()
        {
            // Arrange
            var movingAverageTradeCreator = new MovingAverageTradeCreator();
            var movingAverageTradeParameters = new MovingAverageTradeParameters
            {
                MovingAvarageRange = 5,
                BuyIndicator = 123.45m,
                SellIndicator = 125.67m
            };

            // Act
            movingAverageTradeCreator.Reset(movingAverageTradeParameters);
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 1 });
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 2 });
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 3 });
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 4 });
            movingAverageTradeCreator.CreateTrade(new Trade { Price = 5 });

            // Assert
            Assert.AreEqual(movingAverageTradeCreator.GetCurrentPosition(), 4);
            Assert.AreEqual(movingAverageTradeCreator.GetMovingAvarageRange(), movingAverageTradeParameters.MovingAvarageRange);
            Assert.AreEqual(movingAverageTradeCreator.GetBuyIndicator(), movingAverageTradeParameters.BuyIndicator);
            Assert.AreEqual(movingAverageTradeCreator.GetSellIndicator(), movingAverageTradeParameters.SellIndicator);

            var range = movingAverageTradeCreator.GetRange();
            Assert.AreEqual(range.Length, movingAverageTradeParameters.MovingAvarageRange);
            Assert.AreEqual(range[0], 1);
            Assert.AreEqual(range[1], 2);
            Assert.AreEqual(range[2], 3);
            Assert.AreEqual(range[3], 4);
            Assert.AreEqual(range[4], 5);
        }

        [TestMethod]
        public void MovingAverageTradeCreator_CreateTrades_Reset()
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
            var trade1 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 1 });
            var trade2 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 2 });
            var trade3 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 3 });

            // Act
            movingAverageTradeParameters.MovingAvarageRange = 10;
            movingAverageTradeCreator.Reset(movingAverageTradeParameters);
            var trade4 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 4 });
            var trade5 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 5 });
            var trade6 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 6 });
            var trade7 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 7 });
            var trade8 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 8 });

            // Assert
            Assert.AreEqual(movingAverageTradeCreator.GetCurrentPosition(), 7);
            Assert.AreEqual(movingAverageTradeCreator.GetMovingAvarageRange(), movingAverageTradeParameters.MovingAvarageRange);
            Assert.AreEqual(movingAverageTradeCreator.GetBuyIndicator(), movingAverageTradeParameters.BuyIndicator);
            Assert.AreEqual(movingAverageTradeCreator.GetSellIndicator(), movingAverageTradeParameters.SellIndicator);

            var range = movingAverageTradeCreator.GetRange();
            Assert.AreEqual(range.Length, movingAverageTradeParameters.MovingAvarageRange);
            Assert.AreEqual(range[0], 1);
            Assert.AreEqual(range[1], 2);
            Assert.AreEqual(range[2], 3);
            Assert.AreEqual(range[3], 4);
            Assert.AreEqual(range[4], 5);
            Assert.AreEqual(range[5], 6);
            Assert.AreEqual(range[6], 7);
            Assert.AreEqual(range[7], 8);
            Assert.AreEqual(range[8], 0);
            Assert.AreEqual(range[9], 0);
        }
    }
}
