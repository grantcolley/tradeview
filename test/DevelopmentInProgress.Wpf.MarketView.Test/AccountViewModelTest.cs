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
                    Assert.Fail();
                }
            });

            // Act
            accountViewModel.SetAccount(account);

            // Assert
            Assert.AreSame(account, accountViewModel.Account);
            Assert.IsNull(notifyAccount);
        }

        [TestMethod]
        public void Login_AccountKeyMissing_NotifyHasException()
        {
            // Arrange
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
                    Assert.Fail();
                }
            });

            // Act
            accountViewModel.LoginCommand.Execute(null);

            // Assert
            Assert.AreEqual(errorMessage, "Both api key and secret key are required to login to an account.");
        }

        [TestMethod]
        public void Login()
        {
            // Arrange
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
                    Assert.Fail();
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
        }

        [TestMethod]
        public void SelectedAsset()
        {
            // Arrange
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
                if (args.HasException)
                {
                    //TradeViewModelException(args);
                }
                else if (args.AccountEventType.Equals(AccountEventType.LoggedIn)
                        || args.AccountEventType.Equals(AccountEventType.LoggedOut))
                {
                    //OrdersViewModel.SetAccount(args.Value);
                }
                else if (args.AccountEventType.Equals(AccountEventType.UpdateOrders))
                {
                    //OrdersViewModel.UpdateOrders(args.Value);
                }
                else if (args.AccountEventType.Equals(AccountEventType.SelectedAsset))
                {
                    //TradeViewModel.SetAccount(args.Value, args.SelectedAsset);
                }
            });

            // Act
            accountViewModel.SetAccount(account);


            // Assert
        }
    }
}
