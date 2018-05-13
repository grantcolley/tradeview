using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.MarketView.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.MarketView.Test
{
    [TestClass]
    public class ExchangeServiceTest
    {
        [TestMethod]
        public async Task GetSymbols24HourStatisticsAsync()
        {
            // Arrange
            var exchangeApi = TestExchangeHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var cxlToken = new CancellationToken();

            // Act
            var symbols =  await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken).ConfigureAwait(false);

            // Assert
            var testResults = symbols.Where(s => s.SymbolStatistics.LastPrice.Equals(0.0M)).ToList();
            Assert.IsFalse(testResults.Any());
        }

        [TestMethod]
        public async Task SubscribeStatistics()
        {
            // Arrange
            var exchangeApi = TestExchangeHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var cxlToken = new CancellationToken();

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken).ConfigureAwait(false);

            Action<Exception> exception = e => { };

            // Act
            exchangeService.SubscribeStatistics(symbols, exception, cxlToken);

            // Assert
            var eth = symbols.Single(s => s.Name.Equals("ETHBTC"));
            var updatedEthStats = MarketHelper.EthStats_UpdatedLastPrice_Upwards;

            Assert.AreEqual(eth.SymbolStatistics.PriceChangePercent, updatedEthStats.PriceChangePercent);
            Assert.AreEqual(eth.LastPriceChangeDirection, 1);
            Assert.AreEqual(eth.SymbolStatistics.LastPrice, updatedEthStats.LastPrice);
        }
    }
}
