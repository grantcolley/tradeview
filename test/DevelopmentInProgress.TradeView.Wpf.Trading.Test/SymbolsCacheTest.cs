using DevelopmentInProgress.TradeView.Test.Helper;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.Test
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
            await symbolsCache.GetSymbols(new[] { "BNBBTC" });

            var balances = new List<Core.Model.AccountBalance>();
            balances.Add(new Core.Model.AccountBalance { Asset = "BTC", Free = 0.00794722m });

            var accountInfo = new Core.Model.AccountInfo();
            accountInfo.Balances.AddRange(balances);

            var account = new Account(accountInfo);

            // Act
            symbolsCache.ValueAccount(account);

            // Assert
            Assert.AreEqual(account.USDTValue, 75.14m);
            Assert.AreEqual(account.BTCValue, 0.00794722m);
        }

        [TestMethod]
        public async Task GetSingleBalanceUDTValue_BNB()
        {
            // Arrange
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.Standard);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolsCache = SymbolsCacheHelper.GetSymbolsCache(exchangeService);
            await symbolsCache.GetSymbols(new[] { "BNBBTC" });

            var balances = new List<Core.Model.AccountBalance>();
            balances.Add(new Core.Model.AccountBalance { Asset = "BNB", Free = 1.88373641m });

            var accountInfo = new Core.Model.AccountInfo();
            accountInfo.Balances.AddRange(balances);

            var account = new Account(accountInfo);

            // Act
            symbolsCache.ValueAccount(account);

            // Assert
            Assert.AreEqual(account.USDTValue, 25.93m);
            Assert.AreEqual(account.BTCValue, 0.00274272m);
        }

        [TestMethod]
        public async Task GetMultipleBalancesUDTValue_BTC_BNB()
        {
            // Arrange
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.Standard);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolsCache = SymbolsCacheHelper.GetSymbolsCache(exchangeService);
            await symbolsCache.GetSymbols(new[] { "BNBBTC" });

            var balances = new List<Core.Model.AccountBalance>();
            balances.Add(new Core.Model.AccountBalance { Asset = "BTC", Free = 0.00396715m });
            balances.Add(new Core.Model.AccountBalance { Asset = "BNB", Free = 0.94444141m });

            var accountInfo = new Core.Model.AccountInfo();
            accountInfo.Balances.AddRange(balances);

            var account = new Account(accountInfo);

            // Act
            symbolsCache.ValueAccount(account);
            
            // Assert
            Assert.AreEqual(account.USDTValue, 50.51m);
            Assert.AreEqual(account.BTCValue, 0.00534226m);
        }
    }
}
