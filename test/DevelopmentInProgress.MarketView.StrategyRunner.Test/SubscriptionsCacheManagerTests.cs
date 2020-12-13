//using DevelopmentInProgress.TradeView.Core.Enums;
//using DevelopmentInProgress.TradeView.Core.TradeStrategy;
//using DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers;
//using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Threading.Tasks;

//namespace DevelopmentInProgress.MarketView.StrategyRunner.Test
//{
//    [TestClass]
//    public class SubscriptionsCacheManagerTests
//    {
//        [TestMethod]
//        public void Subscribe_SingleSymbol_MultipleSubscriptions_Subscribe()
//        {
//            // Arrange
//            var exchangeServiceFactory = new TestExchangeServiceFactory();
//            var subscriptionsCacheFactory = new TestSubscriptionsCacheFactory(exchangeServiceFactory);

//            var strategy = new Strategy { Name = "Test" };

//            var trxBinance = new StrategySubscription
//            {
//                Exchange = Exchange.Binance,
//                Symbol = "TRXBTC-BINANCE",
//                Subscribe = (Subscribe.Trades)
//            };

//            var trxTest = new StrategySubscription
//            {
//                Exchange = Exchange.Test,
//                Symbol = "TRXBTC-TEST",
//                Subscribe = (Subscribe.Trades)
//            };

//            strategy.StrategySubscriptions.AddRange(new[] { trxBinance, trxTest });
            
//            var tradeStrategy = new TestTradeStrategy();

//            using (var subscriptionsCacheManager = new SubscriptionsCacheManager(subscriptionsCacheFactory))
//            {
//                // Act
//                subscriptionsCacheManager.Subscribe(strategy, tradeStrategy);

//                // Assert
//                var binance = subscriptionsCacheManager.SubscriptionsCacheFactory.GetExchangeSubscriptionsCache(Exchange.Binance);
                
//                Assert.IsTrue(binance.HasSubscriptions);

//                if (binance.Caches.TryGetValue("TRXBTC-BINANCE", out ISubscriptionCache trxBinanceCache))
//                {
//                    Assert.IsNotNull(trxBinanceCache);
//                    Assert.IsInstanceOfType(trxBinanceCache, typeof(BinanceSymbolSubscriptionCache));
//                    Assert.IsTrue(trxBinanceCache.HasSubscriptions);
//                    Assert.AreEqual(trxBinanceCache.Subscriptions(Subscribe.Trades), 1);
//                }
//                else
//                {
//                    Assert.Fail();
//                }

//                var test = subscriptionsCacheManager.SubscriptionsCacheFactory.GetExchangeSubscriptionsCache(Exchange.Test);

//                Assert.IsTrue(test.HasSubscriptions);

//                if (test.Caches.TryGetValue("TRXBTC-TEST", out ISubscriptionCache trxTestCache))
//                {
//                    Assert.IsNotNull(trxTestCache);
//                    Assert.IsInstanceOfType(trxTestCache, typeof(TestSubscriptionCache));
//                }
//                else
//                {
//                    Assert.Fail();
//                }
//            }
//        }

//        [TestMethod]
//        public async Task Subscribe_SingleSymbol_MultipleSubscriptions_Unsubscribe()
//        {
//            // Arrange
//            var exchangeServiceFactory = new TestExchangeServiceFactory();
//            var subscriptionsCacheFactory = new TestSubscriptionsCacheFactory(exchangeServiceFactory);

//            var strategy = new Strategy { Name = "Test" };

//            var trxBinance = new StrategySubscription
//            {
//                Exchange = Exchange.Binance,
//                Symbol = "TRXBTC-BINANCE",
//                Subscribe = (Subscribe.Trades)
//            };

//            var trxTest = new StrategySubscription
//            {
//                Exchange = Exchange.Test,
//                Symbol = "TRXBTC-TEST",
//                Subscribe = (Subscribe.Trades)
//            };

//            strategy.StrategySubscriptions.AddRange(new[] { trxBinance, trxTest });

//            var tradeStrategy = new TestTradeStrategy();

//            using (var subscriptionsCacheManager = new SubscriptionsCacheManager(subscriptionsCacheFactory))
//            {
//                // Act
//                subscriptionsCacheManager.Subscribe(strategy, tradeStrategy);

//                await Task.Delay(1000);

//                subscriptionsCacheManager.Unsubscribe(strategy, tradeStrategy);

//                // Assert
//                var binance = subscriptionsCacheManager.SubscriptionsCacheFactory.GetExchangeSubscriptionsCache(Exchange.Binance);

//                Assert.IsFalse(binance.HasSubscriptions);

//                var test = subscriptionsCacheManager.SubscriptionsCacheFactory.GetExchangeSubscriptionsCache(Exchange.Test);

//                Assert.IsFalse(test.HasSubscriptions);
//            }
//        }

//        [TestMethod]
//        public void Subscribe_MultipleSymbols_SingleSubscription_Subscribe()
//        {
//            // Arrange
//            var exchangeServiceFactory = new TestExchangeServiceFactory();
//            var subscriptionsCacheFactory = new TestSubscriptionsCacheFactory(exchangeServiceFactory);

//            var strategy1 = new Strategy { Name = "Test 1" };

//            var trxBinance = new StrategySubscription
//            {
//                Exchange = Exchange.Binance,
//                Symbol = "TRXBTC-BINANCE",
//                Subscribe = (Subscribe.Trades)
//            };

//            strategy1.StrategySubscriptions.Add(trxBinance);

//            var strategy2 = new Strategy { Name = "Test 2" };

//            var trxTest = new StrategySubscription
//            {
//                Exchange = Exchange.Test,
//                Symbol = "TRXBTC-TEST",
//                Subscribe = (Subscribe.Trades)
//            };

//            strategy2.StrategySubscriptions.Add(trxTest);

//            var tradeStrategy1 = new TestTradeStrategy();

//            var tradeStrategy2 = new TestTradeStrategy();

//            using (var subscriptionsCacheManager = new SubscriptionsCacheManager(subscriptionsCacheFactory))
//            {
//                // Act
//                subscriptionsCacheManager.Subscribe(strategy1, tradeStrategy1);

//                subscriptionsCacheManager.Subscribe(strategy2, tradeStrategy2);

//                // Assert
//                var binance = subscriptionsCacheManager.SubscriptionsCacheFactory.GetExchangeSubscriptionsCache(Exchange.Binance);

//                Assert.IsTrue(binance.HasSubscriptions);

//                if (binance.Caches.TryGetValue("TRXBTC-BINANCE", out ISubscriptionCache trxBinanceCache))
//                {
//                    Assert.IsNotNull(trxBinanceCache);
//                    Assert.IsInstanceOfType(trxBinanceCache, typeof(BinanceSymbolSubscriptionCache));
//                    Assert.IsTrue(trxBinanceCache.HasSubscriptions);
//                    Assert.AreEqual(trxBinanceCache.Subscriptions(Subscribe.Trades), 1);
//                }
//                else
//                {
//                    Assert.Fail();
//                }

//                var test = subscriptionsCacheManager.SubscriptionsCacheFactory.GetExchangeSubscriptionsCache(Exchange.Test);

//                Assert.IsTrue(test.HasSubscriptions);

//                if (test.Caches.TryGetValue("TRXBTC-TEST", out ISubscriptionCache trxTestCache))
//                {
//                    Assert.IsNotNull(trxTestCache);
//                    Assert.IsInstanceOfType(trxTestCache, typeof(TestSubscriptionCache));
//                }
//                else
//                {
//                    Assert.Fail();
//                }
//            }
//        }

//        [TestMethod]
//        public async Task Subscribe_MultipleSymbols_SingleSubscription_Unsubscribe()
//        {
//            // Arrange
//            var exchangeServiceFactory = new TestExchangeServiceFactory();
//            var subscriptionsCacheFactory = new TestSubscriptionsCacheFactory(exchangeServiceFactory);

//            var strategy1 = new Strategy { Name = "Test 1" };

//            var trxBinance = new StrategySubscription
//            {
//                Exchange = Exchange.Binance,
//                Symbol = "TRXBTC-BINANCE",
//                Subscribe = (Subscribe.Trades)
//            };

//            strategy1.StrategySubscriptions.Add(trxBinance);

//            var strategy2 = new Strategy { Name = "Test 2" };

//            var trxTest = new StrategySubscription
//            {
//                Exchange = Exchange.Test,
//                Symbol = "TRXBTC-TEST",
//                Subscribe = (Subscribe.Trades)
//            };

//            strategy2.StrategySubscriptions.Add(trxTest);

//            var tradeStrategy1 = new TestTradeStrategy();
//            var tradeStrategy2 = new TestTradeStrategy();

//            using (var subscriptionsCacheManager = new SubscriptionsCacheManager(subscriptionsCacheFactory))
//            {
//                // Act
//                subscriptionsCacheManager.Subscribe(strategy1, tradeStrategy1);

//                subscriptionsCacheManager.Subscribe(strategy2, tradeStrategy2);

//                await Task.Delay(1000);

//                subscriptionsCacheManager.Unsubscribe(strategy1, tradeStrategy1);

//                subscriptionsCacheManager.Unsubscribe(strategy2, tradeStrategy2);

//                // Assert
//                var binance = subscriptionsCacheManager.SubscriptionsCacheFactory.GetExchangeSubscriptionsCache(Exchange.Binance);

//                Assert.IsFalse(binance.HasSubscriptions);

//                var test = subscriptionsCacheManager.SubscriptionsCacheFactory.GetExchangeSubscriptionsCache(Exchange.Test);

//                Assert.IsFalse(test.HasSubscriptions);
//            }
//        }
//    }
//}
