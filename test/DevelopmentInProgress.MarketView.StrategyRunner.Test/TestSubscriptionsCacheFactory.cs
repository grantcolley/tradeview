//using DevelopmentInProgress.MarketView.Interface.Interfaces;
//using DevelopmentInProgress.MarketView.Interface.Strategy;
//using DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers;
//using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache;
//using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.Binance;
//using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.ExchangeService;
//using System.Collections.Generic;

//namespace DevelopmentInProgress.MarketView.StrategyRunner.Test
//{
//    public class TestSubscriptionsCacheFactory : ISubscriptionsCacheFactory
//    {
//        private readonly Dictionary<Exchange, ISubscriptionsCache> exchangeSubscriptionsCache;

//        public TestSubscriptionsCacheFactory(IExchangeServiceFactory<IExchangeService> exchangeServiceFactory)
//        {
//            exchangeSubscriptionsCache = new Dictionary<Exchange, ISubscriptionsCache>();
//            exchangeSubscriptionsCache.Add(Exchange.Binance, new BinanceSubscriptionsCache(exchangeServiceFactory.GetExchangeService(Exchange.Binance)));
//            exchangeSubscriptionsCache.Add(Exchange.Test, new TestSubscriptionsCache(exchangeServiceFactory.GetExchangeService(Exchange.Test)));
//        }

//        public ISubscriptionsCache GetSubscriptionsCache(Exchange exchange)
//        {
//            return exchangeSubscriptionsCache.GetValueOrDefault(exchange);
//        }

//        public void Dispose()
//        {
//        }
//    }
//}
