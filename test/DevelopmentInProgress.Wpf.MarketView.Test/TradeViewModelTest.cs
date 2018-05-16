using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.Wpf.MarketView.Extensions;
using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using DevelopmentInProgress.Wpf.MarketView.ViewModel;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using InterfaceExtensions = DevelopmentInProgress.MarketView.Interface.Extensions;
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

        [TestMethod]
        public async Task SelectedSymbol_HasAccount()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);
            var baseBalance = account.Balances.Single(a => a.Asset.Equals("TRX"));
            var quoteAsset = account.Balances.Single(a => a.Asset.Equals("BTC"));

            tradeViewModel.SetAccount(account, null);

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
            Assert.AreEqual(tradeViewModel.BaseAccountBalance, baseBalance);
            Assert.AreEqual(tradeViewModel.QuoteAccountBalance, quoteAsset);
        }

        [TestMethod]
        public async Task SelectedSymbol_Null()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);

            tradeViewModel.SetAccount(account, null);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            tradeViewModel.SetSymbols(symbols.ToList());
            var trx = tradeViewModel.Symbols.Single(s => s.Name.Equals("TRXBTC"));

            tradeViewModel.SelectedSymbol = trx;

            // Act
            tradeViewModel.SelectedSymbol = null;

            // Assert
            Assert.IsNull(tradeViewModel.SelectedSymbol);
            Assert.AreEqual(tradeViewModel.Quantity, 0);
            Assert.AreEqual(tradeViewModel.Price, 0);
            Assert.AreEqual(tradeViewModel.StopPrice, 0);
            Assert.IsNull(tradeViewModel.BaseAccountBalance);
            Assert.IsNull(tradeViewModel.QuoteAccountBalance);
        }

        [TestMethod]
        public async Task Quantity_and_Price_and_StopPrice_Trim()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            tradeViewModel.SetSymbols(symbols.ToList());
            var trx = tradeViewModel.Symbols.Single(s => s.Name.Equals("TRXBTC"));

            tradeViewModel.SelectedSymbol = trx;

            var quantity = 294.123m;
            var price = 1.123456789m;

            // Act
            tradeViewModel.Quantity = quantity;
            tradeViewModel.Price = price;
            tradeViewModel.StopPrice = price;

            // Assert
            Assert.AreEqual(tradeViewModel.Quantity, quantity.Trim(trx.QuantityPrecision));
            Assert.AreEqual(tradeViewModel.Price, price.Trim(trx.PricePrecision));
            Assert.AreEqual(tradeViewModel.StopPrice, price.Trim(trx.PricePrecision));
        }

        [TestMethod]
        public async Task Quantity_and_Price_and_StopPrice_NoTrim()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            tradeViewModel.SetSymbols(symbols.ToList());
            var trx = tradeViewModel.Symbols.Single(s => s.Name.Equals("TRXBTC"));

            tradeViewModel.SelectedSymbol = trx;

            var quantity = 294m;
            var price = 1.12345678m;

            // Act
            tradeViewModel.Quantity = quantity;
            tradeViewModel.Price = price;
            tradeViewModel.StopPrice = price;

            // Assert
            Assert.AreEqual(tradeViewModel.Quantity, quantity.Trim(trx.QuantityPrecision));
            Assert.AreEqual(tradeViewModel.Price, price.Trim(trx.PricePrecision));
            Assert.AreEqual(tradeViewModel.StopPrice, price.Trim(trx.PricePrecision));
        }


        [TestMethod]
        public void Quantity_and_Price_and_StopPrice_NoSelectedSymbol()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var quantity = 294.123m;
            var price = 1.123456789m;

            // Act
            tradeViewModel.Quantity = quantity;
            tradeViewModel.Price = price;
            tradeViewModel.StopPrice = price;

            // Assert
            Assert.AreEqual(tradeViewModel.Quantity, quantity);
            Assert.AreEqual(tradeViewModel.Price, price);
            Assert.AreEqual(tradeViewModel.StopPrice, price);
        }
        
        [TestMethod]
        public async Task SelectedOrderType_SelectedSymbol()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            tradeViewModel.SetSymbols(symbols.ToList());
            var trx = tradeViewModel.Symbols.Single(s => s.Name.Equals("TRXBTC"));

            tradeViewModel.SelectedSymbol = trx;

            // Act
            tradeViewModel.SelectedOrderType = "Limit";

            // Assert
            Assert.AreEqual(tradeViewModel.SelectedOrderType, "Limit");
            Assert.AreEqual(tradeViewModel.Price, trx.SymbolStatistics.LastPrice);
            Assert.AreEqual(tradeViewModel.StopPrice, trx.SymbolStatistics.LastPrice);
        }

        [TestMethod]
        public async Task SelectedOrderType_IsMarketOrder_Not_IsLoading()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            tradeViewModel.SetSymbols(symbols.ToList());
            var trx = tradeViewModel.Symbols.Single(s => s.Name.Equals("TRXBTC"));

            tradeViewModel.SelectedSymbol = trx;

            // Act
            tradeViewModel.SelectedOrderType = "Market";

            // Assert
            Assert.IsFalse(tradeViewModel.IsPriceEditable);
            Assert.IsTrue(tradeViewModel.IsMarketPrice);
        }

        [TestMethod]
        public async Task SelectedOrderType_IsMarketOrder_IsLoading()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            tradeViewModel.SetSymbols(symbols.ToList());
            var trx = tradeViewModel.Symbols.Single(s => s.Name.Equals("TRXBTC"));

            tradeViewModel.IsLoading = true;
            tradeViewModel.SelectedSymbol = trx;

            // Act
            tradeViewModel.SelectedOrderType = "Market";

            // Assert
            Assert.IsFalse(tradeViewModel.IsPriceEditable);
            Assert.IsFalse(tradeViewModel.IsMarketPrice);
        }

        [TestMethod]
        public async Task SelectedOrderType_IsStopLoss_Not_IsLoading()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            tradeViewModel.SetSymbols(symbols.ToList());
            var trx = tradeViewModel.Symbols.Single(s => s.Name.Equals("TRXBTC"));

            tradeViewModel.SelectedSymbol = trx;

            // Act
            tradeViewModel.SelectedOrderType = "Stop Loss";

            // Assert
            Assert.IsFalse(tradeViewModel.IsPriceEditable);
            Assert.IsTrue(tradeViewModel.IsMarketPrice);
        }

        [TestMethod]
        public async Task SelectedOrderType_IsStopLoss_IsLoading()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            tradeViewModel.SetSymbols(symbols.ToList());
            var trx = tradeViewModel.Symbols.Single(s => s.Name.Equals("TRXBTC"));

            tradeViewModel.IsLoading = true;
            tradeViewModel.SelectedSymbol = trx;

            // Act
            tradeViewModel.SelectedOrderType = "Stop Loss";

            // Assert
            Assert.IsFalse(tradeViewModel.IsPriceEditable);
            Assert.IsFalse(tradeViewModel.IsMarketPrice);
        }

        [TestMethod]
        public void HasQuoteBaseBalance_Null()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);
            
            // Act

            // Assert
            Assert.IsFalse(tradeViewModel.HasBaseBalance);
            Assert.IsFalse(tradeViewModel.HasQuoteBalance);
        }

        [TestMethod]
        public void HasQuoteBaseBalance_Zero()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var quoteBalance = new AccountBalance { Free = 0m };
            var baseBalance = new AccountBalance { Free = 0m };

            // Act
            tradeViewModel.QuoteAccountBalance = quoteBalance;
            tradeViewModel.BaseAccountBalance = baseBalance;

            // Assert
            Assert.IsFalse(tradeViewModel.HasBaseBalance);
            Assert.IsFalse(tradeViewModel.HasQuoteBalance);
        }

        [TestMethod]
        public void HasQuoteBaseBalance()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var quoteBalance = new AccountBalance { Free = 299m };
            var baseBalance = new AccountBalance { Free = 0.000123m };

            // Act
            tradeViewModel.QuoteAccountBalance = quoteBalance;
            tradeViewModel.BaseAccountBalance = baseBalance;

            // Assert
            Assert.IsTrue(tradeViewModel.HasBaseBalance);
            Assert.IsTrue(tradeViewModel.HasQuoteBalance);
        }

        [TestMethod]
        public async Task OrderTypes_NoSelectedSymbol()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);

            tradeViewModel.SetSymbols(symbols.ToList());

            // Act
            var orderTypes = tradeViewModel.OrderTypes;

            // Assert
            Assert.IsNull(tradeViewModel.SelectedSymbol);

            var intersection = tradeViewModel.OrderTypes.Intersect(InterfaceExtensions.OrderExtensions.OrderTypes()).ToList();
            Assert.IsTrue(InterfaceExtensions.OrderExtensions.OrderTypes().Count().Equals(intersection.Count));
        }

        [TestMethod]
        public async Task OrderTypes_SelectedSymbol()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);

            var trx = symbols.Single(s => s.Name.Equals("TRXBTC"));
            tradeViewModel.SetSymbols(symbols.ToList());
            tradeViewModel.SelectedSymbol = trx;

            // Act
            var orderTypes = tradeViewModel.OrderTypes;

            // Assert
            Assert.AreEqual(tradeViewModel.SelectedSymbol, trx);
            
            var missing = InterfaceExtensions.OrderExtensions.OrderTypes().Except(tradeViewModel.OrderTypes).ToList();
            foreach(var orderType in missing)
            {
                if (orderType != InterfaceExtensions.OrderExtensions.GetOrderTypeName(Interface.OrderType.StopLoss)
                    && orderType != InterfaceExtensions.OrderExtensions.GetOrderTypeName(Interface.OrderType.TakeProfit))
                {
                    Assert.Fail();
                }
            }
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
