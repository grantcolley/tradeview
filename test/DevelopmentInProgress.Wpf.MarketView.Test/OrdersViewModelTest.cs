using DevelopmentInProgress.MarketView.Test.Helper;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using DevelopmentInProgress.Wpf.MarketView.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevelopmentInProgress.Wpf.MarketView.Model;
using System.Linq;
using System.Threading.Tasks;
using DevelopmentInProgress.Wpf.MarketView.Extensions;
using System;

namespace DevelopmentInProgress.Wpf.MarketView.Test
{
    [TestClass]
    public class OrdersViewModelTest
    {
        [TestMethod]
        public async Task SetAccount_NewAccount()
        {
            // Arrange
            var exchangeApi = ExchangeServiceHelper.GetExchangeService();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var accountViewModel = new OrdersViewModel(exchangeService);

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            //Act
            await accountViewModel.SetAccount(account);
            
            // Assert
            Assert.AreEqual(accountViewModel.Account, account);
            foreach (var order in TestHelper.Orders)
            {
                Assert.IsNotNull(accountViewModel.Orders.SingleOrDefault(o => o.ClientOrderId.Equals(order.ClientOrderId)));
            }
        }

        [TestMethod]
        public async Task SetAccount_OverrideAccount()
        {
            // Arrange
            var exchangeApi = ExchangeServiceHelper.GetExchangeService();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var accountViewModel = new OrdersViewModel(exchangeService);

            var firstAccount = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            accountViewModel.Account = firstAccount;
            var firstorder = new Order { ClientOrderId = "test123", Symbol = "ETHBTC" };
            accountViewModel.Orders.Add(firstorder);

            var newAccount = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "newapikey",
                ApiSecret = "newapisecret"
            };

            // Act
            await accountViewModel.SetAccount(newAccount);

            // Assert
            Assert.IsTrue(accountViewModel.Account.ApiKey.Equals(newAccount.ApiKey));
            foreach (var order in TestHelper.Orders)
            {
                Assert.IsNotNull(accountViewModel.Orders.SingleOrDefault(o => o.ClientOrderId.Equals(order.ClientOrderId)));
            }
        }

        [TestMethod]
        public async Task SetAccount_NullAccount()
        {
            // Arrange
            var exchangeApi = ExchangeServiceHelper.GetExchangeService();
            var exchangeService = new WpfExchangeService(exchangeApi);
            var accountViewModel = new OrdersViewModel(exchangeService);

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            accountViewModel.Account = account;
            foreach (var order in TestHelper.Orders)
            {
                accountViewModel.Orders.Add(order.GetViewOrder());
            }

            //Act
            await accountViewModel.SetAccount(null);

            // Assert
            Assert.IsNull(accountViewModel.Account);
            Assert.IsTrue(accountViewModel.Orders.Count().Equals(0));
        }

        [TestMethod]
        public async Task UpdateOrders()
        {
            // Arrange
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.UpdateOrders);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var accountViewModel = new OrdersViewModel(exchangeService);

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            accountViewModel.Account = account;
            foreach (var order in TestHelper.Orders)
            {
                accountViewModel.Orders.Add(order.GetViewOrder());
            }

            //Act
            await accountViewModel.UpdateOrders(account);

            // Assert
            foreach(var order in accountViewModel.Orders)
            {
                Assert.AreEqual(order.ExecutedQuantity, Math.Round(order.OriginalQuantity * 0.5m, 0));
                Assert.AreEqual(order.FilledPercent, (int)((order.ExecutedQuantity / order.OriginalQuantity) * 100));
            }
        }
    }
}
