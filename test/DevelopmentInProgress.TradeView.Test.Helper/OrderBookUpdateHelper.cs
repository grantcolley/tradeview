using DevelopmentInProgress.TradeView.Interface.Model;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Test.Helper
{
    public class OrderBookUpdateHelper
    {
        public OrderBook OrderBook_Trx_GetFirstUpdate()
        {
            var ask0 = new OrderBookPriceLevel { Price = 1.11m, Quantity = 170 };
            var ask1 = new OrderBookPriceLevel { Price = 1.12m, Quantity = 100 };
            var ask2 = new OrderBookPriceLevel { Price = 1.15m, Quantity = 200 };
            var ask3 = new OrderBookPriceLevel { Price = 1.16m, Quantity = 150 };
            var ask4 = new OrderBookPriceLevel { Price = 1.18m, Quantity = 110 };
            var ask5 = new OrderBookPriceLevel { Price = 1.20m, Quantity = 170 };
            var ask6 = new OrderBookPriceLevel { Price = 1.21m, Quantity = 100 };
            var ask7 = new OrderBookPriceLevel { Price = 1.22m, Quantity = 200 };
            var ask8 = new OrderBookPriceLevel { Price = 1.23m, Quantity = 150 };
            var ask9 = new OrderBookPriceLevel { Price = 1.24m, Quantity = 110 };

            var bid0 = new OrderBookPriceLevel { Price = 1.08m, Quantity = 180 };
            var bid1 = new OrderBookPriceLevel { Price = 1.07m, Quantity = 120 };
            var bid2 = new OrderBookPriceLevel { Price = 1.05m, Quantity = 130 };
            var bid3 = new OrderBookPriceLevel { Price = 1.04m, Quantity = 140 };
            var bid4 = new OrderBookPriceLevel { Price = 1.01m, Quantity = 160 };
            var bid5 = new OrderBookPriceLevel { Price = 1.00m, Quantity = 180 };
            var bid6 = new OrderBookPriceLevel { Price = 0.99m, Quantity = 120 };
            var bid7 = new OrderBookPriceLevel { Price = 0.96m, Quantity = 130 };
            var bid8 = new OrderBookPriceLevel { Price = 0.95m, Quantity = 140 };
            var bid9 = new OrderBookPriceLevel { Price = 0.94m, Quantity = 160 };

            var orderBook = new OrderBook();
            orderBook.Symbol = "TRXBTC";
            orderBook.LastUpdateId = DateTime.Now.Ticks;
            orderBook.Asks = new List<OrderBookPriceLevel>(new[] { ask0, ask1, ask2, ask3, ask4, ask5, ask6, ask7, ask8, ask9 });
            orderBook.Bids = new List<OrderBookPriceLevel>(new[] { bid0, bid1, bid2, bid3, bid4, bid5, bid6, bid7, bid8, bid9 });

            return orderBook;
        }

        public OrderBook OrderBook_Trx_GetSecondUpdate()
        {
            var ask0 = new OrderBookPriceLevel { Price = 1.10m, Quantity = 150 };
            var ask1 = new OrderBookPriceLevel { Price = 1.11m, Quantity = 140 };
            var ask2 = new OrderBookPriceLevel { Price = 1.13m, Quantity = 213 };
            var ask3 = new OrderBookPriceLevel { Price = 1.14m, Quantity = 150 };
            var ask4 = new OrderBookPriceLevel { Price = 1.15m, Quantity = 110 };
            var ask5 = new OrderBookPriceLevel { Price = 1.17m, Quantity = 170 };
            var ask6 = new OrderBookPriceLevel { Price = 1.19m, Quantity = 150 };
            var ask7 = new OrderBookPriceLevel { Price = 1.23m, Quantity = 130 };
            var ask8 = new OrderBookPriceLevel { Price = 1.24m, Quantity = 240 };
            var ask9 = new OrderBookPriceLevel { Price = 1.25m, Quantity = 310 };

            var bid0 = new OrderBookPriceLevel { Price = 1.09m, Quantity = 110 };
            var bid1 = new OrderBookPriceLevel { Price = 1.08m, Quantity = 100 };
            var bid2 = new OrderBookPriceLevel { Price = 1.06m, Quantity = 397 };
            var bid3 = new OrderBookPriceLevel { Price = 1.05m, Quantity = 230 };
            var bid4 = new OrderBookPriceLevel { Price = 1.03m, Quantity = 420 };
            var bid5 = new OrderBookPriceLevel { Price = 1.02m, Quantity = 190 };
            var bid6 = new OrderBookPriceLevel { Price = 1.01m, Quantity = 125 };
            var bid7 = new OrderBookPriceLevel { Price = 0.98m, Quantity = 132 };
            var bid8 = new OrderBookPriceLevel { Price = 0.97m, Quantity = 155 };
            var bid9 = new OrderBookPriceLevel { Price = 0.96m, Quantity = 146 };

            var orderBook = new OrderBook();
            orderBook.Symbol = "TRXBTC";
            orderBook.LastUpdateId = DateTime.Now.Ticks;
            orderBook.Asks = new List<OrderBookPriceLevel>(new[] { ask0, ask1, ask2, ask3, ask4, ask5, ask6, ask7, ask8, ask9 });
            orderBook.Bids = new List<OrderBookPriceLevel>(new[] { bid0, bid1, bid2, bid3, bid4, bid5, bid6, bid7, bid8, bid9 });

            return orderBook;
        }
    }
}
