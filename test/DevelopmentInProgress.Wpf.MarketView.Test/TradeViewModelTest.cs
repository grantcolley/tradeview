using System;
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
using System.Reactive.Linq;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.MarketView.Interface.Validation;

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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            
            // Act
            tradeViewModel.SetAccount(account);

            // Assert
            Assert.AreEqual(tradeViewModel.Account, account);
            Assert.AreEqual(tradeViewModel.SelectedOrderType, null);
            Assert.IsNull(tradeViewModel.SelectedSymbol);
        }

        [TestMethod]
        public async Task SetSymbol()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var asset = account.Balances.Single(a => a.Asset.Equals("TRX"));
            tradeViewModel.SetAccount(account);
            
            // Act
            tradeViewModel.SetSymbol(asset);

            // Assert
            Assert.AreEqual(tradeViewModel.Account, account);
            Assert.AreEqual(tradeViewModel.SelectedOrderType, null);
            Assert.AreEqual(tradeViewModel.SelectedSymbol.Name, trx.Name);
        }

        [TestMethod]
        public async Task SetAccount_Different_Account_Null_SelectedAsset()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);

            tradeViewModel.SetSymbols(symbols.ToList());

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);

            tradeViewModel.SetAccount(account);

            var differentAccount = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "test123",
                ApiSecret = "test123"
            };

            // Act
            tradeViewModel.SetAccount(differentAccount);

            // Assert
            Assert.AreEqual(tradeViewModel.Account, differentAccount);
            Assert.AreEqual(tradeViewModel.SelectedOrderType, null);
            Assert.IsNull(tradeViewModel.SelectedSymbol);
        }

        [TestMethod]
        public async Task SelectedSymbol_NoAccount()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);
            var baseBalance = account.Balances.Single(a => a.Asset.Equals("TRX"));
            var quoteAsset = account.Balances.Single(a => a.Asset.Equals("BTC"));

            tradeViewModel.SetAccount(account);

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
            var exchangeService = new WpfExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);

            tradeViewModel.SetAccount(account);

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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
            var exchangeService = new WpfExchangeService(exchangeApi);
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
        public async Task BuyQuantity_InsufficientFunds()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            var trx = symbols.Single(s => s.Name.Equals("TRXBTC"));

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);
            var selectedAsset = account.Balances.Single(ab => ab.Asset.Equals("TRX"));

            tradeViewModel.SetSymbols(symbols.ToList());
            tradeViewModel.SetAccount(account);
            tradeViewModel.SetSymbol(selectedAsset);

            // Act
            tradeViewModel.BuyQuantityCommand.Execute(75);

            // Assert
            Assert.IsTrue(tradeViewModel.Quantity.Equals(0));
        }

        [TestMethod]
        public async Task BuyQuantity_SufficientFunds()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            var trx = symbols.Single(s => s.Name.Equals("TRXBTC"));

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);
            var selectedAsset = account.Balances.Single(ab => ab.Asset.Equals("TRX"));

            tradeViewModel.SetSymbols(symbols.ToList());
            tradeViewModel.SetAccount(account);
            tradeViewModel.SetSymbol(selectedAsset);
            tradeViewModel.QuoteAccountBalance.Free = 0.00012693M;

            // Act
            tradeViewModel.BuyQuantityCommand.Execute(75);

            // Assert
            Assert.IsTrue(tradeViewModel.Quantity.Equals(10));
        }

        [TestMethod]
        public async Task SellQuantity()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            var trx = symbols.Single(s => s.Name.Equals("TRXBTC"));

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);
            var selectedAsset = account.Balances.Single(ab => ab.Asset.Equals("TRX"));

            tradeViewModel.SetSymbols(symbols.ToList());
            tradeViewModel.SetAccount(account);
            tradeViewModel.SetSymbol(selectedAsset);

            // Act
            tradeViewModel.SellQuantityCommand.Execute(75);

            // Assert
            Assert.IsTrue(tradeViewModel.Quantity.Equals((selectedAsset.Free*0.75m).Trim(trx.QuantityPrecision)));
        }

        [TestMethod]
        public async Task Buy_Pass()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            var trx = symbols.Single(s => s.Name.Equals("TRXBTC"));

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);
            var selectedAsset = account.Balances.Single(ab => ab.Asset.Equals("TRX"));

            tradeViewModel.SetSymbols(symbols.ToList());
            tradeViewModel.SetAccount(account);
            tradeViewModel.SetSymbol(selectedAsset);
            
            tradeViewModel.SelectedOrderType = "Limit";
            tradeViewModel.Quantity = 200m;
            tradeViewModel.Price = 0.00000900M;
            tradeViewModel.QuoteAccountBalance.Free = 200m * 0.00000900M;

            var tradeObservable = Observable.FromEventPattern<TradeEventArgs>(
                eventHandler => tradeViewModel.OnTradeNotification += eventHandler,
                eventHandler => tradeViewModel.OnTradeNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            Exception ex = null;
            tradeObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    ex = args.Exception;
                }
            });

            // Act
            tradeViewModel.BuyCommand.Execute(null);

            // Assert
            Assert.IsNull(ex);
        }

        [TestMethod]
        public async Task Buy_Fails_No_OrderType()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            var trx = symbols.Single(s => s.Name.Equals("TRXBTC"));

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);
            var selectedAsset = account.Balances.Single(ab => ab.Asset.Equals("TRX"));

            tradeViewModel.SetSymbols(symbols.ToList());
            tradeViewModel.SetAccount(account);
            tradeViewModel.SetSymbol(selectedAsset);
            
            tradeViewModel.Quantity = 200m;
            tradeViewModel.Price = 0.00000900M;

            var tradeObservable = Observable.FromEventPattern<TradeEventArgs>(
                eventHandler => tradeViewModel.OnTradeNotification += eventHandler,
                eventHandler => tradeViewModel.OnTradeNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            Exception ex = null;
            tradeObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    ex = args.Exception;
                }
            });

            // Act
            tradeViewModel.BuyCommand.Execute(null);

            // Assert
            Assert.IsNotNull(ex);
            Assert.IsTrue(ex.Message.Contains("order type"));
        }

        [TestMethod]
        public async Task Buy_Fails_Order_Validation()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            var trx = symbols.Single(s => s.Name.Equals("TRXBTC"));

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);
            var selectedAsset = account.Balances.Single(ab => ab.Asset.Equals("TRX"));

            tradeViewModel.SetSymbols(symbols.ToList());
            tradeViewModel.SetAccount(account);
            tradeViewModel.SetSymbol(selectedAsset);

            tradeViewModel.QuoteAccountBalance.Free = 0.00012693M;

            tradeViewModel.SelectedOrderType = "Limit";
            tradeViewModel.Price = 0.00000900M;

            var tradeObservable = Observable.FromEventPattern<TradeEventArgs>(
                eventHandler => tradeViewModel.OnTradeNotification += eventHandler,
                eventHandler => tradeViewModel.OnTradeNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            Exception ex = null;
            tradeObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    ex = args.Exception;
                }
            });

            // Act
            tradeViewModel.BuyCommand.Execute(null);

            // Assert
            Assert.IsInstanceOfType(ex, typeof(OrderValidationException));
        }

        [TestMethod]
        public async Task Buy_Fails_PlaceOrder()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi(ExchangeApiType.PlaceOrderException);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            var trx = symbols.Single(s => s.Name.Equals("TRXBTC"));

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);
            var selectedAsset = account.Balances.Single(ab => ab.Asset.Equals("TRX"));

            tradeViewModel.SetSymbols(symbols.ToList());
            tradeViewModel.SetAccount(account);
            tradeViewModel.SetSymbol(selectedAsset);

            tradeViewModel.QuoteAccountBalance.Free = 0.00012693M;

            tradeViewModel.SelectedOrderType = "Limit";
            tradeViewModel.Quantity = 200m;
            tradeViewModel.Price = 0.00000900M;
            tradeViewModel.QuoteAccountBalance.Free = 200m * 0.00000900M;

            var tradeObservable = Observable.FromEventPattern<TradeEventArgs>(
                eventHandler => tradeViewModel.OnTradeNotification += eventHandler,
                eventHandler => tradeViewModel.OnTradeNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            Exception ex = null;
            tradeObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    ex = args.Exception;
                }
            });

            // Act
            tradeViewModel.BuyCommand.Execute(null);

            // Assert
            Assert.IsNotNull(ex);
            Assert.IsTrue(ex.Message.Contains("failed to place order"));
        }

        [TestMethod]
        public async Task Sell_Pass()
        {
            // Arrange
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeApiHelper.GetExchangeApi();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var tradeViewModel = new TradeViewModel(exchangeService);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);
            var trx = symbols.Single(s => s.Name.Equals("TRXBTC"));

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            account = await exchangeService.GetAccountInfoAsync(account.AccountInfo.User.ApiKey, account.AccountInfo.User.ApiSecret, cxlToken);
            var selectedAsset = account.Balances.Single(ab => ab.Asset.Equals("TRX"));

            tradeViewModel.SetSymbols(symbols.ToList());
            tradeViewModel.SetAccount(account);
            tradeViewModel.SetSymbol(selectedAsset);

            tradeViewModel.QuoteAccountBalance.Free = 0.00012693M;

            tradeViewModel.SelectedOrderType = "Limit";
            tradeViewModel.Quantity = tradeViewModel.BaseAccountBalance.Free;
            tradeViewModel.Price = 0.00000850M;

            var tradeObservable = Observable.FromEventPattern<TradeEventArgs>(
                eventHandler => tradeViewModel.OnTradeNotification += eventHandler,
                eventHandler => tradeViewModel.OnTradeNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            Exception ex = null;
            tradeObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    ex = args.Exception;
                }
            });

            // Act
            tradeViewModel.SellCommand.Execute(null);

            // Assert
            Assert.IsNull(ex);
        }
    }
}