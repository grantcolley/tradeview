using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public class HelperFactoryContainer : IHelperFactoryContainer
    {
        private readonly Dictionary<Type, IHelperFactory> factories;

        public HelperFactoryContainer(ITradeHelperFactory tradeHelperFactory, IOrderBookHelperFactory orderBookHelperFactory)
        {
            factories = new Dictionary<Type, IHelperFactory>
            {
                { typeof(ITradeHelperFactory), tradeHelperFactory },
                { typeof(IOrderBookHelperFactory), orderBookHelperFactory }
            };
        }

        public T GetFactory<T>() where T : IHelperFactory
        {
            return (T)factories[typeof(T)];
        }
    }
}
