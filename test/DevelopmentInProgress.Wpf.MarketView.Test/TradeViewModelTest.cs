using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using DevelopmentInProgress.Wpf.MarketView.ViewModel;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.Wpf.MarketView.Test
{
    [TestClass]
    public class TradeViewModelTest
    {
        [TestMethod]
        public async Task SetAccount()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);

            var trx = symbols.Single(s => s.Name.Equals("TRXBTC"));

            tradeViewModel.SetSymbols(symbols.ToList());

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);

            var selectedAsset = account.Balances.Single(ab => ab.Asset.Equals("TRX"));

            // Act
            tradeViewModel.SetAccount(account, selectedAsset);

            // Assert
            Assert.AreEqual(tradeViewModel.Account, account);
            Assert.AreEqual(tradeViewModel.SelectedOrderType, string.Empty);
            Assert.AreEqual(tradeViewModel.SelectedSymbol, trx);
        }

        [TestMethod]
        public async Task SetAccount_Different_Account_Null_SelectedAsset()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);

            tradeViewModel.SetSymbols(symbols.ToList());

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);

            var selectedAsset = account.Balances.Single(ab => ab.Asset.Equals("TRX"));

            tradeViewModel.SetAccount(account, selectedAsset);

            var differentAccount = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "test123",
                ApiSecret = "test123"
            };

            // Act
            tradeViewModel.SetAccount(differentAccount, null);

            // Assert
            Assert.AreEqual(tradeViewModel.Account, differentAccount);
            Assert.AreEqual(tradeViewModel.SelectedOrderType, string.Empty);
            Assert.IsNull(tradeViewModel.SelectedSymbol);
        }

        [TestMethod]
        public async Task SelectedSymbol_NoAccount()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            tradeViewModel.SetSymbols(symbols.ToList());
            var trx = tradeViewModel.Symbols.Single(s => s.Name.Equals("TRXBTC"));
            
            // Act
            tradeViewModel.SelectedSymbol = trx;

            // Assert
            Assert.AreEqual(tradeViewModel.SelectedSymbol, trx);
            Assert.AreEqual(tradeViewModel.Quantity, 0);
            Assert.AreEqual(tradeViewModel.Price, trx.SymbolStatistics.LastPrice);
            Assert.AreEqual(tradeViewModel.StopPrice, trx.SymbolStatistics.LastPrice);
            Assert.IsNull(tradeViewModel.BaseAccountBalance);
            Assert.IsNull(tradeViewModel.QuoteAccountBalance);
        }

        //[TestMethod]
        //public async Task SelectedSymbol_HasAccount()
        //{
        //    // Arrange
        //    var cxlToken = new CancellationToken();
        //    var exchangeApi = ExchangeApiHelper.GetExchangeApi();
        //    var exchangeService = new ExchangeService(exchangeApi);
        //    var tradeViewModel = new TradeViewModel(exchangeService);

        //    var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
        //    {
        //        ApiKey = "apikey",
        //        ApiSecret = "apisecret"
        //    };

        //    account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);

        //    //tradeViewModel.SetAccount(account)

        //    var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
        //    tradeViewModel.SetSymbols(symbols.ToList());
        //    var trx = tradeViewModel.Symbols.Single(s => s.Name.Equals("TRXBTC"));

        //    // Act
        //    tradeViewModel.SelectedSymbol = trx;

        //    // Assert
        //    Assert.AreEqual(tradeViewModel.SelectedSymbol, trx);
        //    Assert.AreEqual(tradeViewModel.Quantity, 0);
        //    Assert.AreEqual(tradeViewModel.Price, trx.SymbolStatistics.LastPrice);
        //    Assert.AreEqual(tradeViewModel.StopPrice, trx.SymbolStatistics.LastPrice);
        //    Assert.IsNull(tradeViewModel.BaseAccountBalance);
        //    Assert.IsNull(tradeViewModel.QuoteAccountBalance);
        //}

        [TestMethod]
        public void Quantity_and_Price()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void SelectedOrderType()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void IsPriceEditable()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void IsMarketPrice()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void IsStopLoss()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void HasBaseBalance()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void HasQuoteBalance()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void OrderTypes()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void BuyQuantity()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void SellQuantity()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void SetQuantity()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Buy()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Sell()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void SendClientOrder()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }
    }
}
