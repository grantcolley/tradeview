using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using DevelopmentInProgress.Wpf.MarketView.ViewModel;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reactive.Linq;
using Newtonsoft.Json;

namespace DevelopmentInProgress.Wpf.MarketView.Test
{
    [TestClass]
    public class AccountViewModelTest
    {
        [TestMethod]
        public void SetAccount()
        {
            // Arrange
            var fail = false;
            Account notifyAccount = null;
            var exchangeApi = TestExchangeHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var accountViewModel = new AccountViewModel(exchangeService);

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            var accountObservable = Observable.FromEventPattern<AccountEventArgs>(
                eventHandler => accountViewModel.OnAccountNotification += eventHandler,
                eventHandler => accountViewModel.OnAccountNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            accountObservable.Subscribe(args =>
            {
                if (args.AccountEventType.Equals(AccountEventType.LoggedOut))
                {
                    notifyAccount = args.Value;
                }
                else
                {
                    fail = true;
                }
            });

            // Act
            accountViewModel.SetAccount(account);

            // Assert
            Assert.AreSame(account, accountViewModel.Account);
            Assert.IsNull(notifyAccount);
            Assert.IsFalse(fail);
        }

        [TestMethod]
        public void Login_AccountKeyMissing_NotifyHasException()
        {
            // Arrange
            var fail = false;
            string errorMessage = string.Empty;
            var exchangeApi = TestExchangeHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var accountViewModel = new AccountViewModel(exchangeService);

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey"
            };

            accountViewModel.Account = account;

            var accountObservable = Observable.FromEventPattern<AccountEventArgs>(
                eventHandler => accountViewModel.OnAccountNotification += eventHandler,
                eventHandler => accountViewModel.OnAccountNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            accountObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    errorMessage = args.Exception.Message;
                }
                else
                {
                    fail = true;
                }
            });

            // Act
            accountViewModel.LoginCommand.Execute(null);

            // Assert
            Assert.AreEqual(errorMessage, "Both api key and secret key are required to login to an account.");
            Assert.IsFalse(fail);
        }

        [TestMethod]
        public void Login()
        {
            // Arrange
            var fail = false;
            var loggedInAccount = string.Empty;
            string errorMessage = string.Empty;
            var exchangeApi = TestExchangeHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var accountViewModel = new AccountViewModel(exchangeService);

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            accountViewModel.Account = account;

            var accountObservable = Observable.FromEventPattern<AccountEventArgs>(
                eventHandler => accountViewModel.OnAccountNotification += eventHandler,
                eventHandler => accountViewModel.OnAccountNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            accountObservable.Subscribe(args =>
            {
                if (args.AccountEventType.Equals(AccountEventType.LoggedIn))
                {
                    loggedInAccount = JsonConvert.SerializeObject(args.Value);
                }
                else
                {
                    fail = true;
                }
            });

            // Act
            accountViewModel.LoginCommand.Execute(null);

            // Assert
            var accountViewModelAccount = JsonConvert.SerializeObject(accountViewModel.Account);
            var accountInfo = JsonConvert.SerializeObject(accountViewModel.Account.AccountInfo);
            var testAccountInfo = JsonConvert.SerializeObject(MarketHelper.AccountInfo);

            Assert.AreEqual(accountViewModelAccount, loggedInAccount);
            Assert.AreEqual(accountInfo, testAccountInfo);
            Assert.IsFalse(fail);
        }

        [TestMethod]
        public void Login_AccountInfo_Update()
        {
            // Arrange
            var fail = false;
            var hasUpdated = false;
            string errorMessage = string.Empty;
            var exchangeApi = TestExchangeHelper.GetExchangeApi(ExchangeApiType.SubscribeAccountInfo);
            var exchangeService = new ExchangeService(exchangeApi);
            var accountViewModel = new AccountViewModel(exchangeService);

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            accountViewModel.Account = account;

            var accountObservable = Observable.FromEventPattern<AccountEventArgs>(
                eventHandler => accountViewModel.OnAccountNotification += eventHandler,
                eventHandler => accountViewModel.OnAccountNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            accountObservable.Subscribe(args =>
            {
                if (args.AccountEventType.Equals(AccountEventType.LoggedIn))
                {
                    // expected
                }
                else if (args.AccountEventType.Equals(AccountEventType.UpdateOrders))
                {
                    hasUpdated = true;
                }
                else
                {
                    fail = true;
                }
            });

            // Act
            accountViewModel.LoginCommand.Execute(null);

            // Assert
            var btc = accountViewModel.Account.Balances.SingleOrDefault(ab => ab.Asset.Equals("BTC"));
            var bcpt = accountViewModel.Account.Balances.SingleOrDefault(ab => ab.Asset.Equals("BCPT"));
            var test = accountViewModel.Account.Balances.SingleOrDefault(ab => ab.Asset.Equals("TEST"));

            Assert.IsTrue(hasUpdated);
            Assert.IsNull(btc);
            Assert.IsNull(bcpt);
            Assert.IsNotNull(test);
            Assert.IsTrue(accountViewModel.Account.Balances.Count().Equals(5));
            Assert.IsFalse(fail);
        }

        [TestMethod]
        public void SelectedAsset()
        {
            // Arrange
            var fail = false;
            AccountBalance selectedAccountBalance = null;
            var exchangeApi = TestExchangeHelper.GetExchangeApi();
            var exchangeService = new ExchangeService(exchangeApi);
            var accountViewModel = new AccountViewModel(exchangeService);

            var account = new Account(new Interface.AccountInfo { User = new Interface.User() })
            {
                ApiKey = "apikey",
                ApiSecret = "apisecret"
            };

            accountViewModel.Account = account;

            var accountObservable = Observable.FromEventPattern<AccountEventArgs>(
                eventHandler => accountViewModel.OnAccountNotification += eventHandler,
                eventHandler => accountViewModel.OnAccountNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            accountObservable.Subscribe(args =>
            {
                if (args.AccountEventType.Equals(AccountEventType.LoggedOut)
                    || args.AccountEventType.Equals(AccountEventType.LoggedIn))
                {
                    // expected
                }
                else if (args.AccountEventType.Equals(AccountEventType.SelectedAsset))
                {
                    selectedAccountBalance = args.SelectedAsset;
                }
                else
                {
                    fail = true;
                }
            });

            // Act
            accountViewModel.SetAccount(account);
            accountViewModel.LoginCommand.Execute(null);
            var trx = accountViewModel.Account.Balances.Single(ab => ab.Asset.Equals("TRX"));
            accountViewModel.SelectedAsset = trx;

            // Assert
            Assert.AreEqual(selectedAccountBalance, trx);
            Assert.IsFalse(fail);
        }
    }
}
