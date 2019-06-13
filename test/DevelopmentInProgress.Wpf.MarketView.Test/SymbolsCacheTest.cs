using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.MarketView.Test
{
    [TestClass]
    public class SymbolsCacheTest
    {
        [TestMethod]
        public async Task GetSingleBalanceUDTValue_BTC()
        {
            // Arrange
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.Standard);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolsCache = SymbolsCacheHelper.GetSymbolsCache(exchangeService);
            var symbols = await symbolsCache.GetSymbols();

            var balances = new List<AccountBalance>();
            balances.Add(new AccountBalance { Asset = "BTC", Free = 0.00794722m });

            // Act
            var usdtValue = symbolsCache.USDTValueBalances(balances);

            // Assert
            Assert.AreEqual(usdtValue, 64.84m);
        }

        [TestMethod]
        public async Task GetSingleBalanceUDTValue_BNB()
        {
            // Arrange
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.Standard);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolsCache = SymbolsCacheHelper.GetSymbolsCache(exchangeService);
            var symbols = await symbolsCache.GetSymbols();

            var balances = new List<AccountBalance>();
            balances.Add(new AccountBalance { Asset = "BNB", Free = 1.88373641m });

            // Act
            var usdtValue = symbolsCache.USDTValueBalances(balances);

            // Assert
            Assert.AreEqual(usdtValue, 64.57m);
        }

        [TestMethod]
        public async Task GetMultipleBalancesUDTValue_BTC_BNB()
        {
            // Arrange
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.Standard);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolsCache = SymbolsCacheHelper.GetSymbolsCache(exchangeService);
            var symbols = await symbolsCache.GetSymbols();

            var balances = new List<AccountBalance>();
            balances.Add(new AccountBalance { Asset = "BTC", Free = 0.00396715m });
            balances.Add(new AccountBalance { Asset = "BNB", Free = 0.94444141m });

            // Act
            var usdtValue = symbolsCache.USDTValueBalances(balances);

            // Assert
            Assert.AreEqual(usdtValue, 64.74m);
        }
    }
}
