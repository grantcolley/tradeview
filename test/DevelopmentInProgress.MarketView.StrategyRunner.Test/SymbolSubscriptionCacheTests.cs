using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Model;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test
{
    [TestClass]
    public class SymbolSubscriptionCacheTests
    {
        [TestMethod]
        public async Task Subscribe_AggregateTrades_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.Trades };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            symbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            // Assert
            Assert.IsTrue(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.Trades), 1);
            Assert.IsNotNull(tradeStrategy.AggregateTrades);
            Assert.IsTrue(tradeStrategy.AggregateTrades.Any());
        }

        [TestMethod]
        public async Task Subscribe_AggregateTrades_Multiple_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.Trades };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.Trades };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            // Act
            symbolCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

            symbolCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(2000);

            // Assert
            Assert.IsTrue(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.Trades), 2);
            Assert.IsNotNull(tradeStrategy1.AggregateTrades);
            Assert.IsTrue(tradeStrategy1.AggregateTrades.Any());
            Assert.IsNotNull(tradeStrategy2.AggregateTrades);
            Assert.IsTrue(tradeStrategy2.AggregateTrades.Any());
        }

        [TestMethod]
        public async Task Subscribe_AggregateTrades_Single_Subscriber_Unsubscribe()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.Trades };
            var tradeStrategy = new TestTradeStrategy();

            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            // Act
            symbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            symbolCache.Unsubscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            // Assert
            Assert.IsFalse(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.Trades), 0);
            Assert.IsNotNull(tradeStrategy.AggregateTrades);
            Assert.IsTrue(tradeStrategy.AggregateTrades.Any());
        }

        [TestMethod]
        public async Task Subscribe_AggregateTrades_Multiple_Subscribers_Unsubscribe_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.Trades };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.Trades };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            // Act
            symbolCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

            symbolCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(2000);

            symbolCache.Unsubscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(1000);

            // Assert
            Assert.IsTrue(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.Trades), 1);
            Assert.IsNotNull(tradeStrategy1.AggregateTrades);
            Assert.IsTrue(tradeStrategy1.AggregateTrades.Any());
            Assert.IsNotNull(tradeStrategy2.AggregateTrades);
            Assert.IsTrue(tradeStrategy2.AggregateTrades.Any());
        }

        [TestMethod]
        public async Task Subscribe_AggregateTrades_Multiple_Subscribers_Unsubscribe_All_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.Trades };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.Trades };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            // Act
            symbolCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

            symbolCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(2000);

            symbolCache.Unsubscribe("Test 2", strategySubscription2, tradeStrategy2);

            symbolCache.Unsubscribe("Test 1", strategySubscription1, tradeStrategy1);

            await Task.Delay(1000);

            // Assert
            Assert.IsFalse(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.Trades), 0);
            Assert.IsNotNull(tradeStrategy1.AggregateTrades);
            Assert.IsTrue(tradeStrategy1.AggregateTrades.Any());
            Assert.IsNotNull(tradeStrategy2.AggregateTrades);
            Assert.IsTrue(tradeStrategy2.AggregateTrades.Any());
        }

        [TestMethod]
        public async Task AggregateTrades_Exception()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi { AggregateTradesException = true };
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.Trades };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            symbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            // Assert
            Assert.IsTrue(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.Trades), 1);
            Assert.IsNotNull(tradeStrategy.AggregateTrades);
            Assert.IsTrue(tradeStrategy.AggregateTrades.Any());
            Assert.IsTrue(tradeStrategy.AggregateTradesException);
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.OrderBook };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            symbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            // Assert
            Assert.IsTrue(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.OrderBook), 1);
            Assert.IsNotNull(tradeStrategy.OrderBook);
            Assert.IsTrue(tradeStrategy.OrderBook.Asks.Any());
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_Multiple_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.OrderBook };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.OrderBook };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            // Act
            symbolCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

            symbolCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(2000);

            // Assert
            Assert.IsTrue(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.OrderBook), 2);
            Assert.IsNotNull(tradeStrategy1.OrderBook);
            Assert.IsTrue(tradeStrategy1.OrderBook.Asks.Any());
            Assert.IsNotNull(tradeStrategy2.OrderBook);
            Assert.IsTrue(tradeStrategy2.OrderBook.Asks.Any());
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_Single_Subscriber_Unsubscribe()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.OrderBook };
            var tradeStrategy = new TestTradeStrategy();

            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            // Act
            symbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            symbolCache.Unsubscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            // Assert
            Assert.IsFalse(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.OrderBook), 0);
            Assert.IsNotNull(tradeStrategy.OrderBook);
            Assert.IsTrue(tradeStrategy.OrderBook.Asks.Any());
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_Multiple_Subscribers_Unsubscribe_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.OrderBook };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.OrderBook };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            // Act
            symbolCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

            symbolCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(2000);

            symbolCache.Unsubscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(1000);

            // Assert
            Assert.IsTrue(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.OrderBook), 1);
            Assert.IsNotNull(tradeStrategy1.OrderBook);
            Assert.IsTrue(tradeStrategy1.OrderBook.Asks.Any());
            Assert.IsNotNull(tradeStrategy2.OrderBook);
            Assert.IsTrue(tradeStrategy2.OrderBook.Asks.Any());
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_Multiple_Subscribers_Unsubscribe_All_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.OrderBook };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.OrderBook };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            // Act
            symbolCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

            symbolCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(2000);

            symbolCache.Unsubscribe("Test 2", strategySubscription2, tradeStrategy2);

            symbolCache.Unsubscribe("Test 1", strategySubscription1, tradeStrategy1);

            await Task.Delay(1000);

            // Assert
            Assert.IsFalse(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.OrderBook), 0);
            Assert.IsNotNull(tradeStrategy1.OrderBook);
            Assert.IsTrue(tradeStrategy1.OrderBook.Asks.Any());
            Assert.IsNotNull(tradeStrategy2.OrderBook);
            Assert.IsTrue(tradeStrategy2.OrderBook.Asks.Any());
        }

        [TestMethod]
        public async Task OrderBook_Exception()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi { OrderBookException = true };
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.OrderBook };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            symbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            // Assert
            Assert.IsTrue(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.OrderBook), 1);
            Assert.IsNotNull(tradeStrategy.OrderBook);
            Assert.IsTrue(tradeStrategy.OrderBook.Asks.Any());
            Assert.IsTrue(tradeStrategy.OrderBookException);
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_AggregateTrades_Multiple_Subscribers_Unsubscribe_Some()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = (Subscribes.OrderBook | Subscribes.Trades) };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = (Subscribes.OrderBook | Subscribes.Trades) };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            // Act
            symbolCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

            symbolCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(2000);

            symbolCache.Unsubscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(1000);

            // Assert
            Assert.IsTrue(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.OrderBook), 1);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.Trades), 1);
            Assert.IsNotNull(tradeStrategy1.OrderBook);
            Assert.IsTrue(tradeStrategy1.OrderBook.Asks.Any());
            Assert.IsNotNull(tradeStrategy1.AggregateTrades);
            Assert.IsTrue(tradeStrategy1.AggregateTrades.Any());
            Assert.IsNotNull(tradeStrategy2.OrderBook);
            Assert.IsTrue(tradeStrategy2.OrderBook.Asks.Any());
            Assert.IsNotNull(tradeStrategy2.AggregateTrades);
            Assert.IsTrue(tradeStrategy2.AggregateTrades.Any());
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_AggregateTrades_Multiple_Subscribers_Unsubscribe_All()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = (Subscribes.OrderBook | Subscribes.Trades) };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = (Subscribes.OrderBook | Subscribes.Trades) };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            // Act
            symbolCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

            symbolCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

            await Task.Delay(2000);

            symbolCache.Unsubscribe("Test 2", strategySubscription2, tradeStrategy1);

            symbolCache.Unsubscribe("Test 1", strategySubscription1, tradeStrategy2);

            await Task.Delay(1000);

            // Assert
            Assert.IsFalse(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.OrderBook), 0);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.Trades), 0);
            Assert.IsNotNull(tradeStrategy1.OrderBook);
            Assert.IsTrue(tradeStrategy1.OrderBook.Asks.Any());
            Assert.IsNotNull(tradeStrategy1.AggregateTrades);
            Assert.IsTrue(tradeStrategy1.AggregateTrades.Any());
            Assert.IsNotNull(tradeStrategy2.OrderBook);
            Assert.IsTrue(tradeStrategy2.OrderBook.Asks.Any());
            Assert.IsNotNull(tradeStrategy2.AggregateTrades);
            Assert.IsTrue(tradeStrategy2.AggregateTrades.Any());
        }
    }
}
