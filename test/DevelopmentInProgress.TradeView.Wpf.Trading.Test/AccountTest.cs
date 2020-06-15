using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.Test
{
    [TestClass]
    public class AccountTest
    {
        private static string apiKey;
        private static Core.Model.User user;
        private static List<Core.Model.AccountBalance>  balances;

        [ClassInitialize()]
        public static void ClientOrderValidationBuilderTest_Initialize(TestContext testContext)
        {
            apiKey = "abcdefghijklmnopqrstuvwxyz";
            user = new Core.Model.User() { ApiKey = apiKey };
            var balance = new Core.Model.AccountBalance { Asset = "TRX", Free = 300, Locked = 100 };
            balances = new List<Core.Model.AccountBalance>() { balance };
        }

        [TestMethod]
        public void CreateAccountInstance_Pass()
        {
            // Arrange
            var accountInfo = new Core.Model.AccountInfo { User = user };
            accountInfo.Balances.AddRange(balances);

            // Act
            var account = new Account(accountInfo);

            // Assert
            Assert.IsTrue(account.ApiKey.Equals(apiKey));
            Assert.IsTrue(account.Balances.Count.Equals(1));
            Assert.IsNotNull(account.Balances.Single(b => b.Asset.Equals("TRX")));
            Assert.IsTrue(string.IsNullOrWhiteSpace(account.ApiSecret));
        }

        [TestMethod]
        public void SetApiSecret_Pass()
        {
            // Arrange
            var account = new Account(new Core.Model.AccountInfo { User = user });

            // Act
            account.ApiSecret = "0123456789";

            // Assert
            Assert.IsTrue(account.ApiSecret.Equals("**********"));
            Assert.IsTrue(account.AccountInfo.User.ApiSecret.Equals("0123456789"));;
        }
    }
}
