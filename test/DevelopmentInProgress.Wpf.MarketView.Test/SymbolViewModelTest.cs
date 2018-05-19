using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.MarketView.Extensions;
using DevelopmentInProgress.Wpf.MarketView.Services;
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
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var symbolViewModel = new SymbolViewModel(exchangeService);

            var trx = TestHelper.Trx.GetViewSymbol();
            
            // Act
            await symbolViewModel.SetSymbol(trx);

            // Assert
            Assert.AreEqual(symbolViewModel.Symbol, trx);
            Assert.IsNotNull(symbolViewModel.OrderBook);
            Assert.AreEqual(symbolViewModel.OrderBook.LastUpdateId, TestHelper.OrderBook.LastUpdateId);
            Assert.IsNotNull(symbolViewModel.OrderBook.Top);
            Assert.IsTrue(symbolViewModel.OrderBook.Asks.Count > 0);
            Assert.IsTrue(symbolViewModel.OrderBook.Bids.Count > 0);
            Assert.IsTrue(symbolViewModel.AggregateTrades.Count > 0);
        }

        [TestMethod]
        public async Task UpdateOrderBook()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi(ExchangeApiType.SubscribeOrderBookAggregateTrades);
            var exchangeService = new ExchangeService(exchangeApi);
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
            var exchangeApi = ExchangeApiHelper.GetExchangeApi(ExchangeApiType.SubscribeOrderBookAggregateTrades);
            var exchangeService = new ExchangeService(exchangeApi);
            var symbolViewModel = new SymbolViewModel(exchangeService);

            var trx = TestHelper.Trx.GetViewSymbol();

            // Act
            await symbolViewModel.SetSymbol(trx);

            // Assert
        }
    }
}
