using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Test.Helper;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevelopmentInProgress.TradeView.Interface.Enums;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.Test
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

            var symbols = await exchangeService.GetSymbols24HourStatisticsAsync(Exchange.Test, cxlToken);

            var orderTypes = new List<Interface.Model.OrderType>();
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
