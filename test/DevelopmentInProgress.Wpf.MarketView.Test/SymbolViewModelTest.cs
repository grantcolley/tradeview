using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
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
            Assert.IsNotNull(symbolViewModel.OrderBook.Top);
            Assert.IsTrue(symbolViewModel.OrderBook.Asks.Count > 0);
            Assert.IsTrue(symbolViewModel.OrderBook.Bids.Count > 0);
        }

        [TestMethod]
        public void GetOrderBook()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void UpdateOrderBook()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void GetTrades()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void UpdateAggregateTrades()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}
