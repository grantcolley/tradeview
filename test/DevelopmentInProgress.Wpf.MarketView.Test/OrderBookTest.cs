using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.MarketView.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.MarketView.Test
{
    [TestClass]
    public class OrderBookTest
    {
        [TestMethod]
        public void OrderBook_Update()
        {
            // Arrange
            var orderBook = new OrderBook();
            var data = TestHelper.OrderBook;

            var asks = new List<OrderBookPriceLevel>();
            var ten = new OrderBookPriceLevel { Price = 10, Quantity = 1000 };
            var seven = new OrderBookPriceLevel { Price = 7, Quantity = 700 };
            var nine = new OrderBookPriceLevel { Price = 9, Quantity = 900 };
            asks.AddRange(new[] { ten, seven, nine });

            var bids = new List<OrderBookPriceLevel>();
            var three = new OrderBookPriceLevel { Price = 3, Quantity = 300 };
            var one = new OrderBookPriceLevel { Price = 1, Quantity = 100 };
            var four = new OrderBookPriceLevel { Price = 4, Quantity = 400 };
            bids.AddRange(new[] { three, one, four });
            
            // Act
            orderBook.Update(asks, bids);

            // Assert
            Assert.AreEqual(orderBook.TopAsks[0], ten);
            Assert.AreEqual(orderBook.TopAsks[1], nine);
            Assert.AreEqual(orderBook.TopAsks[2], seven);

            Assert.AreEqual(orderBook.TopBids[0], four);
            Assert.AreEqual(orderBook.TopBids[1], three);
            Assert.AreEqual(orderBook.TopBids[2], one);

            Assert.AreEqual(orderBook.ChartAsks[0], seven);
            Assert.AreEqual(orderBook.ChartAsks[1], nine);
            Assert.AreEqual(orderBook.ChartAsks[2], ten);

            Assert.AreEqual(orderBook.ChartBids[0], four);
            Assert.AreEqual(orderBook.ChartBids[1], three);
            Assert.AreEqual(orderBook.ChartBids[2], one);

            Assert.AreEqual(orderBook.ChartAggregatedAsks[0].Price, seven.Price);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[0].Quantity, seven.Quantity);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[1].Price, nine.Price);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[1].Quantity, seven.Quantity + nine.Quantity);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[2].Price, ten.Price);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[2].Quantity, seven.Quantity + nine.Quantity + ten.Quantity);

            Assert.AreEqual(orderBook.ChartAggregatedBids[0].Price, four.Price);
            Assert.AreEqual(orderBook.ChartAggregatedBids[0].Quantity, four.Quantity);
            Assert.AreEqual(orderBook.ChartAggregatedBids[1].Price, three.Price);
            Assert.AreEqual(orderBook.ChartAggregatedBids[1].Quantity, four.Quantity + three.Quantity);
            Assert.AreEqual(orderBook.ChartAggregatedBids[2].Price, one.Price);
            Assert.AreEqual(orderBook.ChartAggregatedBids[2].Quantity, four.Quantity + three.Quantity + one.Quantity);
        }

        [TestMethod]
        public void OrderBook_UpdateWithChanges()
        {
            // Arrange
            var orderBook = new OrderBook();
            var data = TestHelper.OrderBook;

            var asks = new List<OrderBookPriceLevel>();
            var ten = new OrderBookPriceLevel { Price = 10, Quantity = 1000 };
            var seven = new OrderBookPriceLevel { Price = 7, Quantity = 700 };
            var nine = new OrderBookPriceLevel { Price = 9, Quantity = 900 };
            asks.AddRange(new[] { ten, seven, nine });

            var bids = new List<OrderBookPriceLevel>();
            var three = new OrderBookPriceLevel { Price = 3, Quantity = 300 };
            var one = new OrderBookPriceLevel { Price = 1, Quantity = 100 };
            var four = new OrderBookPriceLevel { Price = 4, Quantity = 400 };
            bids.AddRange(new[] { three, one, four });

            orderBook.Update(asks, bids);

            asks = new List<OrderBookPriceLevel>();
            var six = new OrderBookPriceLevel { Price = 6, Quantity = 600 };
            seven.Quantity = 7100;
            var eight = new OrderBookPriceLevel { Price = 8, Quantity = 800 };

            asks.AddRange(new[] { six, seven, eight });

            bids = new List<OrderBookPriceLevel>();
            var two = new OrderBookPriceLevel { Price = 2, Quantity = 200 };
            one.Quantity = 1100;
            var five = new OrderBookPriceLevel { Price = 5, Quantity = 500 };
            bids.AddRange(new[] { two, one, five });

            // Act
            orderBook.Update(asks, bids);

            // Assert
            Assert.AreEqual(orderBook.TopAsks[0], eight);
            Assert.AreEqual(orderBook.TopAsks[1], seven);
            Assert.AreEqual(orderBook.TopAsks[2], six);

            Assert.AreEqual(orderBook.TopBids[0], five);
            Assert.AreEqual(orderBook.TopBids[1], two);
            Assert.AreEqual(orderBook.TopBids[2], one);

            Assert.AreEqual(orderBook.ChartAsks[0], six);
            Assert.AreEqual(orderBook.ChartAsks[1], seven);
            Assert.AreEqual(orderBook.ChartAsks[2], eight);

            Assert.AreEqual(orderBook.ChartBids[0], five);
            Assert.AreEqual(orderBook.ChartBids[1], two);
            Assert.AreEqual(orderBook.ChartBids[2], one);

            Assert.AreEqual(orderBook.ChartAggregatedAsks[0].Price, six.Price);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[0].Quantity, six.Quantity);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[1].Price, seven.Price);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[1].Quantity, six.Quantity + seven.Quantity);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[2].Price, eight.Price);
            Assert.AreEqual(orderBook.ChartAggregatedAsks[2].Quantity, six.Quantity + seven.Quantity + eight.Quantity);

            Assert.AreEqual(orderBook.ChartAggregatedBids[0].Price, five.Price);
            Assert.AreEqual(orderBook.ChartAggregatedBids[0].Quantity, five.Quantity);
            Assert.AreEqual(orderBook.ChartAggregatedBids[1].Price, two.Price);
            Assert.AreEqual(orderBook.ChartAggregatedBids[1].Quantity, five.Quantity + two.Quantity);
            Assert.AreEqual(orderBook.ChartAggregatedBids[2].Price, one.Price);
            Assert.AreEqual(orderBook.ChartAggregatedBids[2].Quantity, five.Quantity + two.Quantity + one.Quantity);
        }
    }
}
