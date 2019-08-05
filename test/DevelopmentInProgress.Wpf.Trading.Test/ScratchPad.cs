using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevelopmentInProgress.Wpf.Trading.Test
{
    [TestClass]
    public class ScratchPad
    {
        [TestMethod]
        public async Task GetRangeOfOrderTypes()
        {
            var cxlToken = new CancellationToken();
            var exchangeApi = ExchangeServiceHelper.GetExchangeService();
            var exchangeService = new WpfExchangeService(exchangeApi);

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(cxlToken);

            var orderTypes = new List<Interface.OrderType>();
            foreach(var symbol in symbols)
            {
                foreach(var orderType in symbol.OrderTypes)
                {
                    if(!orderTypes.Contains(orderType))
                    {
                        orderTypes.Add(orderType);
                    }
                }
            }

            Assert.AreEqual(orderTypes.Count, 5);
        }
    }
}
