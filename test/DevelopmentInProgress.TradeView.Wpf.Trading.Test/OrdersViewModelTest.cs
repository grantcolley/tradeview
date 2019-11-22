using DevelopmentInProgress.TradeView.Test.Helper;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Linq;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Wpf.Common.Extensions;
using System;
using Prism.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.Test
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
            var accountViewModel = new OrdersViewModel(exchangeService, new DebugLogger());

            var account = new Account(new Interface.Model.AccountInfo { User = new Interface.Model.User() })
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
            var accountViewModel = new OrdersViewModel(exchangeService, new DebugLogger());

            var firstAccount = new Account(new Interface.Model.AccountInfo { User = new Interface.Model.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            accountViewModel.Account = firstAccount;
            var firstorder = new Order { ClientOrderId = "test123", Symbol = "ETHBTC" };
            accountViewModel.Orders.Add(firstorder);

            var newAccount = new Account(new Interface.Model.AccountInfo { User = new Interface.Model.User() })
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
            var accountViewModel = new OrdersViewModel(exchangeService, new DebugLogger());

            var account = new Account(new Interface.Model.AccountInfo { User = new Interface.Model.User() })
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
            var accountViewModel = new OrdersViewModel(exchangeService, new DebugLogger());

            var account = new Account(new Interface.Model.AccountInfo { User = new Interface.Model.User() })
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
