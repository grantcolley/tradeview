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

        [TestMethod]
        public void GetMovingAverage_PartialRange()
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
            var trade1 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 25.40m });
            var trade2 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 25.90m });
            var trade3 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 26.50m });

            // Assert
            Assert.AreEqual(trade1.MovingAveragePrice, 25.40m);
            Assert.AreEqual(trade2.MovingAveragePrice, 25.65m);
            Assert.AreEqual(Math.Truncate(100 * trade3.MovingAveragePrice) / 100, 25.93m); //truncate to 2 decimal places
        }

        [TestMethod]
        public void GetMovingAverage_FullRange()
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
            var trade1 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 25.40m });
            var trade2 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 25.90m });
            var trade3 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 26.50m });
            var trade4 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 26.30m });
            var trade5 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 27.90m });

            // Assert
            Assert.AreEqual(trade1.MovingAveragePrice, 25.40m);
            Assert.AreEqual(trade2.MovingAveragePrice, 25.65m);
            Assert.AreEqual(Math.Truncate(100 * trade3.MovingAveragePrice) / 100, 25.93m); //truncate to 2 decimal places
            Assert.AreEqual(trade4.MovingAveragePrice, 26.025m);
            Assert.AreEqual(trade5.MovingAveragePrice, 26.40m);
        }

        [TestMethod]
        public void GetMovingAverage_FullRange_After_Reshuffle()
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
            var trade1 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 25.40m });
            var trade2 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 25.90m });
            var trade3 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 26.50m });
            var trade4 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 26.30m });
            var trade5 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 27.90m });
            var trade6 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 27.40m });
            var trade7 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 28.20m });

            // Assert
            Assert.AreEqual(trade1.MovingAveragePrice, 25.40m);
            Assert.AreEqual(trade2.MovingAveragePrice, 25.65m);
            Assert.AreEqual(Math.Truncate(100 * trade3.MovingAveragePrice) / 100, 25.93m); //truncate to 2 decimal places
            Assert.AreEqual(trade4.MovingAveragePrice, 26.025m);
            Assert.AreEqual(trade5.MovingAveragePrice, 26.40m);
            Assert.AreEqual(trade6.MovingAveragePrice, 26.80m);
            Assert.AreEqual(trade7.MovingAveragePrice, 27.26m);
        }

        [TestMethod]
        public void GetMovingAverage_Buy_And_Sell_Indicators_FullRange_After_Reshuffle()
        {
            // Arrange
            var movingAverageTradeCreator = new MovingAverageTradeCreator();
            var movingAverageTradeParameters = new MovingAverageTradeParameters
            {
                MovingAvarageRange = 5,
                BuyIndicator = 0.1m,
                SellIndicator = 0.1m
            };

            movingAverageTradeCreator.Reset(movingAverageTradeParameters);

            // Act
            var trade1 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 25.40m });
            var trade2 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 25.90m });
            var trade3 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 26.50m });
            var trade4 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 26.30m });
            var trade5 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 27.90m });
            var trade6 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 27.40m });
            var trade7 = movingAverageTradeCreator.CreateTrade(new Trade { Price = 28.20m });

            // Assert
            Assert.AreEqual(trade1.MovingAveragePrice, 25.40m);
            Assert.AreEqual(trade1.BuyPrice, trade1.MovingAveragePrice - (trade1.MovingAveragePrice * movingAverageTradeParameters.BuyIndicator));
            Assert.AreEqual(trade1.SellPrice, trade1.MovingAveragePrice + (trade1.MovingAveragePrice * movingAverageTradeParameters.SellIndicator));

            Assert.AreEqual(trade2.MovingAveragePrice, 25.65m);
            Assert.AreEqual(trade2.BuyPrice, trade2.MovingAveragePrice - (trade2.MovingAveragePrice * movingAverageTradeParameters.BuyIndicator));
            Assert.AreEqual(trade2.SellPrice, trade2.MovingAveragePrice + (trade2.MovingAveragePrice * movingAverageTradeParameters.SellIndicator));

            Assert.AreEqual(Math.Truncate(100 * trade3.MovingAveragePrice) / 100, 25.93m); //truncate to 2 decimal places
            Assert.AreEqual(trade3.BuyPrice, trade3.MovingAveragePrice - (trade3.MovingAveragePrice * movingAverageTradeParameters.BuyIndicator));
            Assert.AreEqual(trade3.SellPrice, trade3.MovingAveragePrice + (trade3.MovingAveragePrice * movingAverageTradeParameters.SellIndicator));

            Assert.AreEqual(trade4.MovingAveragePrice, 26.025m);
            Assert.AreEqual(trade4.BuyPrice, trade4.MovingAveragePrice - (trade4.MovingAveragePrice * movingAverageTradeParameters.BuyIndicator));
            Assert.AreEqual(trade4.SellPrice, trade4.MovingAveragePrice + (trade4.MovingAveragePrice * movingAverageTradeParameters.SellIndicator));

            Assert.AreEqual(trade5.MovingAveragePrice, 26.40m);
            Assert.AreEqual(trade5.BuyPrice, trade5.MovingAveragePrice - (trade5.MovingAveragePrice * movingAverageTradeParameters.BuyIndicator));
            Assert.AreEqual(trade5.SellPrice, trade5.MovingAveragePrice + (trade5.MovingAveragePrice * movingAverageTradeParameters.SellIndicator));

            Assert.AreEqual(trade6.MovingAveragePrice, 26.80m);
            Assert.AreEqual(trade6.BuyPrice, trade6.MovingAveragePrice - (trade6.MovingAveragePrice * movingAverageTradeParameters.BuyIndicator));
            Assert.AreEqual(trade6.SellPrice, trade6.MovingAveragePrice + (trade6.MovingAveragePrice * movingAverageTradeParameters.SellIndicator));

            Assert.AreEqual(trade7.MovingAveragePrice, 27.26m);
            Assert.AreEqual(trade7.BuyPrice, trade7.MovingAveragePrice - (trade7.MovingAveragePrice * movingAverageTradeParameters.BuyIndicator));
            Assert.AreEqual(trade7.SellPrice, trade7.MovingAveragePrice + (trade7.MovingAveragePrice * movingAverageTradeParameters.SellIndicator));
        }
    }
}
