using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test
{
    [TestClass]
    public class AccountInfoSubscriptionCacheTests
    {
        [TestMethod]
        public async Task Subscribe_AccountInfo_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Subscribes = Subscribes.AccountInfo };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using var accountInfoCache = new AccountInfoSubscriptionCache(binanceExchangeService);
            accountInfoCache.Subscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            // Assert
            Assert.IsTrue(accountInfoCache.HasSubscriptions);
            Assert.AreEqual(accountInfoCache.Subscriptions(Subscribes.AccountInfo), 1);
            Assert.IsNotNull(tradeStrategy.AccountInfo);
            Assert.IsTrue(tradeStrategy.AccountInfo.Balances.Any());
        }

        [TestMethod]
        public async Task Subscribe_AccountInfo_Multiple_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Subscribes = Subscribes.AccountInfo };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Subscribes = Subscribes.AccountInfo };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using var accountInfoCache = new AccountInfoSubscriptionCache(binanceExchangeService);

            // Act
            accountInfoCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

            accountInfoCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(2000);

            // Assert
            Assert.IsTrue(accountInfoCache.HasSubscriptions);
            Assert.AreEqual(accountInfoCache.Subscriptions(Subscribes.AccountInfo), 2);
            Assert.IsNotNull(tradeStrategy1.AccountInfo);
            Assert.IsTrue(tradeStrategy1.AccountInfo.Balances.Any());
            Assert.IsNotNull(tradeStrategy2.AccountInfo);
            Assert.IsTrue(tradeStrategy2.AccountInfo.Balances.Any());
        }

        [TestMethod]
        public async Task Subscribe_AccountInfo_Single_Subscriber_Unsubscribe()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Subscribes = Subscribes.AccountInfo };
            var tradeStrategy = new TestTradeStrategy();

            using var accountInfoCache = new AccountInfoSubscriptionCache(binanceExchangeService);

            // Act
            accountInfoCache.Subscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            accountInfoCache.Unsubscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            // Assert
            Assert.IsFalse(accountInfoCache.HasSubscriptions);
            Assert.AreEqual(accountInfoCache.Subscriptions(Subscribes.AccountInfo), 0);
            Assert.IsNotNull(tradeStrategy.AccountInfo);
            Assert.IsTrue(tradeStrategy.AccountInfo.Balances.Any());
        }

        [TestMethod]
        public async Task Subscribe_AccountInfo_Multiple_Subscribers_Unsubscribe_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Subscribes = Subscribes.AccountInfo };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Subscribes = Subscribes.AccountInfo };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using var accountInfoCache = new AccountInfoSubscriptionCache(binanceExchangeService);

            // Act
            accountInfoCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

            accountInfoCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(2000);

            accountInfoCache.Unsubscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(1000);

            // Assert
            Assert.IsTrue(accountInfoCache.HasSubscriptions);
            Assert.AreEqual(accountInfoCache.Subscriptions(Subscribes.AccountInfo), 1);
            Assert.IsNotNull(tradeStrategy1.AccountInfo);
            Assert.IsTrue(tradeStrategy1.AccountInfo.Balances.Any());
            Assert.IsNotNull(tradeStrategy2.AccountInfo);
            Assert.IsTrue(tradeStrategy2.AccountInfo.Balances.Any());
        }

        [TestMethod]
        public async Task Subscribe_AccountInfo_Multiple_Subscribers_Unsubscribe_All_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Subscribes = Subscribes.AccountInfo };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Subscribes = Subscribes.AccountInfo };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using var accountInfoCache = new AccountInfoSubscriptionCache(binanceExchangeService);

            // Act
            accountInfoCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

            accountInfoCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(2000);

            accountInfoCache.Unsubscribe("Test 2", strategySubscription2, tradeStrategy2);

            accountInfoCache.Unsubscribe("Test 1", strategySubscription1, tradeStrategy1);

            await Task.Delay(1000);

            // Assert
            Assert.IsFalse(accountInfoCache.HasSubscriptions);
            Assert.AreEqual(accountInfoCache.Subscriptions(Subscribes.AccountInfo), 0);
            Assert.IsNotNull(tradeStrategy1.AccountInfo);
            Assert.IsTrue(tradeStrategy1.AccountInfo.Balances.Any());
            Assert.IsNotNull(tradeStrategy2.AccountInfo);
            Assert.IsTrue(tradeStrategy2.AccountInfo.Balances.Any());
        }

        [TestMethod]
        public async Task AccountInfo_Exception()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi { AccountInfoException = true };
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Subscribes = Subscribes.AccountInfo };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using var accountInfoCache = new AccountInfoSubscriptionCache(binanceExchangeService);
            accountInfoCache.Subscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            // Assert
            Assert.IsTrue(accountInfoCache.HasSubscriptions);
            Assert.AreEqual(accountInfoCache.Subscriptions(Subscribes.AccountInfo), 1);
            Assert.IsNotNull(tradeStrategy.AccountInfo);
            Assert.IsTrue(tradeStrategy.AccountInfo.Balances.Any());
            Assert.IsTrue(tradeStrategy.AccountInfoException);
        }
    }
}
