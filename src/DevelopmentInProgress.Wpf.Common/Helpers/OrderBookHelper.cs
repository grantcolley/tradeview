using DevelopmentInProgress.Wpf.Common.Model;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentInProgress.Wpf.Common.Helpers
{
    public class OrderBookHelper
    {
        public static List<OrderBookPriceLevel> GetAggregatedList(List<OrderBookPriceLevel> orders)
        {
            var count = orders.Count();

            var aggregatedList = orders.Select(p => new OrderBookPriceLevel { Price = p.Price, Quantity = p.Quantity }).ToList();

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    aggregatedList[i].Quantity = aggregatedList[i].Quantity + aggregatedList[i - 1].Quantity;
                }
            }

            return aggregatedList;
        }
    }
}