using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Model;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;
using DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Subscriptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test
{
    [TestClass]
    public class SubscriptionManagerTests
    {
        [TestMethod]
        public async Task AggregateTradeUpdate_Exception_RemainSubscribed_HandleException()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.Trades };
            var tradeStrategy = new TestTradeExceptionStrategy();

            // Act
            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            symbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            // Assert
            Assert.IsTrue(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.Trades), 1);
            Assert.IsTrue(tradeStrategy.AggregateTradesException);
        }

        [TestMethod]
        public async Task OrderBookException_ForciblyUnsubscribed()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribes = Subscribes.OrderBook };
            var tradeStrategy = new TestTradeExceptionStrategy();

            // Act
            using var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService);

            symbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

            await Task.Delay(1000);

            // Assert
            Assert.IsFalse(symbolCache.HasSubscriptions);
            Assert.AreEqual(symbolCache.Subscriptions(Subscribes.OrderBook), 0);
            Assert.IsTrue(tradeStrategy.OrderBookException);
        }
    }
}
