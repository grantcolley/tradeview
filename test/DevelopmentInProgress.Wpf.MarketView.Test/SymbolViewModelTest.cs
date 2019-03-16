using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.Common.Chart;
using DevelopmentInProgress.Wpf.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Services;
using DevelopmentInProgress.Wpf.MarketView.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.Wpf.MarketView.Test
{
    [TestClass]
    public class SymbolViewModelTest
    {
        IChartHelper chartHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            chartHelper = new ChartHelper();
        }

        [TestMethod]
        public async Task SetSymbol()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeServiceHelper.GetExchangeService();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolViewModel = new SymbolViewModel(exchangeService, chartHelper, new Model.Preferences());

            var trx = TestHelper.Trx.GetViewSymbol();
            
            // Act
            await symbolViewModel.SetSymbol(trx);

            // Assert
            Assert.AreEqual(symbolViewModel.Symbol, trx);
            Assert.IsNotNull(symbolViewModel.OrderBook);
            Assert.AreEqual(symbolViewModel.OrderBook.LastUpdateId, TestHelper.OrderBook.LastUpdateId);
            Assert.IsNotNull(symbolViewModel.OrderBook.Top);
            Assert.IsTrue(symbolViewModel.OrderBook.TopAsks.Count > 0);
            Assert.IsTrue(symbolViewModel.OrderBook.TopBids.Count > 0);
            Assert.IsTrue(symbolViewModel.Trades.Count > 0);
        }

        [TestMethod]
        public void UpdateOrderBook_FirstUpdate()
        {
            // Arrange
            var exchangeApi = ExchangeServiceHelper.GetExchangeService();
            var exchangeService = new WpfExchangeService(exchangeApi);

            var preferences = new Model.Preferences();
            preferences.OrderBookChartDisplayCount = 8;
            preferences.OrderBookDisplayCount = 5;
            
            var symbolViewModel = new SymbolViewModel(exchangeService, chartHelper, preferences);

            var trx = TestHelper.Trx.GetViewSymbol();
            symbolViewModel.Symbol = trx;

            var orderBook = OrderBookUpdateHelper.OrderBook_Trx_GetFirstUpdate();
            
            // Act
            symbolViewModel.UpdateOrderBook(orderBook);

            // Assert
            AssertOrderBookUpdate(symbolViewModel, orderBook, preferences);
        }

        private void AssertOrderBookUpdate(SymbolViewModel symbolViewModel, OrderBook orderBook, Model.Preferences preferences)
        {
            // Assert - Last Update Id
            Assert.AreEqual(symbolViewModel.OrderBook.LastUpdateId, orderBook.LastUpdateId);

            // Assert - TopAsks
            Assert.AreEqual(symbolViewModel.OrderBook.TopAsks.Count, preferences.OrderBookDisplayCount);
            var topAsks = orderBook.Asks.Take(preferences.OrderBookDisplayCount).Reverse().ToList();
            for (int i = 0; i < preferences.OrderBookDisplayCount; i++)
            {
                Assert.AreEqual(symbolViewModel.OrderBook.TopAsks[i].Price, topAsks[i].Price);
                Assert.AreEqual(symbolViewModel.OrderBook.TopAsks[i].Quantity, topAsks[i].Quantity);
            }

            // Assert - TopBids
            Assert.AreEqual(symbolViewModel.OrderBook.TopBids.Count, preferences.OrderBookDisplayCount);
            var topBids = orderBook.Bids.Take(preferences.OrderBookDisplayCount).ToList();
            for (int i = 0; i < preferences.OrderBookDisplayCount; i++)
            {
                Assert.AreEqual(symbolViewModel.OrderBook.TopBids[i].Price, topBids[i].Price);
                Assert.AreEqual(symbolViewModel.OrderBook.TopBids[i].Quantity, topBids[i].Quantity);
            }

            // Assert - ChartAsks
            Assert.AreEqual(symbolViewModel.OrderBook.ChartAsks.Count, preferences.OrderBookChartDisplayCount);
            var chartAsks = orderBook.Asks.Take(preferences.OrderBookChartDisplayCount).ToList();
            for (int i = 0; i < preferences.OrderBookChartDisplayCount; i++)
            {
                Assert.AreEqual(symbolViewModel.OrderBook.ChartAsks[i].Price, chartAsks[i].Price);
                Assert.AreEqual(symbolViewModel.OrderBook.ChartAsks[i].Quantity, chartAsks[i].Quantity);
            }

            // Assert ChartBids
            Assert.AreEqual(symbolViewModel.OrderBook.ChartBids.Count, preferences.OrderBookChartDisplayCount);
            var chartBids = orderBook.Bids.Take(preferences.OrderBookChartDisplayCount).Reverse<OrderBookPriceLevel>().ToList();
            for (int i = 0; i < preferences.OrderBookChartDisplayCount; i++)
            {
                Assert.AreEqual(symbolViewModel.OrderBook.ChartBids[i].Price, chartBids[i].Price);
                Assert.AreEqual(symbolViewModel.OrderBook.ChartBids[i].Quantity, chartBids[i].Quantity);
            }

            // Assert ChartAggregateAsks
            Assert.AreEqual(symbolViewModel.OrderBook.ChartAggregatedAsks.Count, preferences.OrderBookChartDisplayCount);
            var runningTotal = 0m;
            for (int i = 0; i < preferences.OrderBookChartDisplayCount; i++)
            {
                if (i == 0)
                {
                    runningTotal = chartAsks[i].Quantity;
                }
                else
                {
                    runningTotal = chartAsks[i].Quantity + runningTotal;
                }

                Assert.AreEqual(symbolViewModel.OrderBook.ChartAggregatedAsks[i].Price, chartAsks[i].Price);
                Assert.AreEqual(symbolViewModel.OrderBook.ChartAggregatedAsks[i].Quantity, runningTotal);
            }

            // Assert ChartAggregateBids
            Assert.AreEqual(symbolViewModel.OrderBook.ChartAggregatedBids.Count, preferences.OrderBookChartDisplayCount);

            var aggregatedBidsList = orderBook.Bids.Take(preferences.OrderBookChartDisplayCount).Select(p => new OrderBookPriceLevel { Price = p.Price, Quantity = p.Quantity }).ToList();
            for (int i = 0; i < preferences.OrderBookChartDisplayCount; i++)
            {
                if (i > 0)
                {
                    aggregatedBidsList[i].Quantity = aggregatedBidsList[i].Quantity + aggregatedBidsList[i - 1].Quantity;
                }
            }

            var reversedAggregateBidsList = aggregatedBidsList.Reverse<OrderBookPriceLevel>().ToList();
            for (int i = 0; i < preferences.OrderBookChartDisplayCount; i++)
            {
                Assert.AreEqual(symbolViewModel.OrderBook.ChartAggregatedBids[i].Price, reversedAggregateBidsList[i].Price);
                Assert.AreEqual(symbolViewModel.OrderBook.ChartAggregatedBids[i].Quantity, reversedAggregateBidsList[i].Quantity);
            }
        }

        [TestMethod]
        public void UpdateOrderBook_SecondUpdate()
        {
            // Arrange
            var exchangeApi = ExchangeServiceHelper.GetExchangeService();
            var exchangeService = new WpfExchangeService(exchangeApi);

            var preferences = new Model.Preferences();
            preferences.OrderBookChartDisplayCount = 8;
            preferences.OrderBookDisplayCount = 5;

            var symbolViewModel = new SymbolViewModel(exchangeService, chartHelper, preferences);

            var trx = TestHelper.Trx.GetViewSymbol();
            symbolViewModel.Symbol = trx;

            var firstOrderBook = OrderBookUpdateHelper.OrderBook_Trx_GetFirstUpdate();
            var secondOrderBook = OrderBookUpdateHelper.OrderBook_Trx_GetSecondUpdate();

            // Act
            symbolViewModel.UpdateOrderBook(firstOrderBook);

            symbolViewModel.UpdateOrderBook(secondOrderBook);

            // Assert
            AssertOrderBookUpdate(symbolViewModel, secondOrderBook, preferences);
        }

        [TestMethod]
        public async Task UpdateTrades()
        {
            throw new NotImplementedException();

            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.SubscribeOrderBookAggregateTrades);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolViewModel = new SymbolViewModel(exchangeService, chartHelper, new Model.Preferences());

            var trx = TestHelper.Trx.GetViewSymbol();

            // Act
            await symbolViewModel.SetSymbol(trx);

            // Assert
            var trades = TestHelper.Trades.Take(symbolViewModel.TradesDisplayCount).ToList();
            var updatedtrades = TestHelper.TradesUpdated;

            var maxId = trades.Max(t => t.Id);          
            var newTrades = (from t in updatedtrades
                               where t.Id > maxId
                               orderby t.Time
                               select t).ToList();

            for(int i = 0; i < newTrades.Count(); i++)
            {
                if (trades.Count >= symbolViewModel.TradesDisplayCount)
                {
                    trades.RemoveAt(trades.Count - 1);
                }

                trades.Insert(0, newTrades[i]);
            }

            Assert.AreEqual(symbolViewModel.Trades.Count, trades.Count);

            for(int i = 0; i < symbolViewModel.Trades.Count; i++)
            {
                Assert.AreEqual(symbolViewModel.Trades[i].Id, trades[i].Id);
            }
        }
    }
}
