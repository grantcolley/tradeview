using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Services;
using DevelopmentInProgress.Wpf.MarketView.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.Wpf.MarketView.Test
{
    [TestClass]
    public class SymbolViewModelTest
    {
        [TestMethod]
        public async Task SetSymbol()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeServiceHelper.GetExchangeService();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolViewModel = new SymbolViewModel(exchangeService);

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
            Assert.IsTrue(symbolViewModel.AggregateTrades.Count > 0);
        }

        [TestMethod]
        public async Task UpdateOrderBook()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.SubscribeOrderBookAggregateTrades);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolViewModel = new SymbolViewModel(exchangeService);

            var trx = TestHelper.Trx.GetViewSymbol();

            // Act
            await symbolViewModel.SetSymbol(trx);

            // Assert
            Assert.AreEqual(symbolViewModel.OrderBook.LastUpdateId, TestHelper.OrderBookUpdated.LastUpdateId);
        }

        [TestMethod]
        public async Task UpdateAggregateTrades()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.SubscribeOrderBookAggregateTrades);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolViewModel = new SymbolViewModel(exchangeService);

            var trx = TestHelper.Trx.GetViewSymbol();

            // Act
            await symbolViewModel.SetSymbol(trx);

            // Assert
            var trades = TestHelper.AggregateTrades.Take(symbolViewModel.TradesDisplayLimit).ToList();
            var updatedtrades = TestHelper.AggregateTradesUpdated;

            var maxId = trades.Max(t => t.Id);          
            var newTrades = (from t in updatedtrades
                               where t.Id > maxId
                               orderby t.Time
                               select t).ToList();

            for(int i = 0; i < newTrades.Count(); i++)
            {
                if (trades.Count >= symbolViewModel.TradesDisplayLimit)
                {
                    trades.RemoveAt(trades.Count - 1);
                }

                trades.Insert(0, newTrades[i]);
            }

            Assert.AreEqual(symbolViewModel.AggregateTrades.Count, trades.Count);

            for(int i = 0; i < symbolViewModel.AggregateTrades.Count; i++)
            {
                Assert.AreEqual(symbolViewModel.AggregateTrades[i].Id, trades[i].Id);
            }
        }
    }
}
