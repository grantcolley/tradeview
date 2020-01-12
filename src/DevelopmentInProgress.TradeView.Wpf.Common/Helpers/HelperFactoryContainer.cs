using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public class HelperFactoryContainer : IHelperFactoryContainer
    {
        private Dictionary<Type, IHelperFactory> factories;

        public HelperFactoryContainer(ITradeHelperFactory tradeHelperFactory, IOrderBookHelperFactory orderBookHelperFactory)
        {
            factories = new Dictionary<Type, IHelperFactory>();
            factories.Add(typeof(ITradeHelperFactory), tradeHelperFactory);
            factories.Add(typeof(IOrderBookHelperFactory), orderBookHelperFactory);
        }

        public IHelperFactory GetFactory<T>() where T : IHelperFactory
        {
            return factories[typeof(T)];
        }
    }
}
