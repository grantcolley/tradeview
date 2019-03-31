using DevelopmentInProgress.MarketView.Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentInProgress.MarketView.Test.Helper
{
    public static class TradesUpdateHelper
    {
        public static List<Trade> Trades_BNB_GetFirstUpdate()
        {
            //    "Symbol":"BNBBTC",
            //    "Id":39316368,
            //    "Price":0.00395100,
            //    "Quantity":20.39000000,
            //    "BuyerOrderId":-1,
            //    "SellerOrderId":-1,
            //    "Time":"2019-03-16T20:07:35.843Z",
            //    "IsBuyerMaker":true,
            //    "IsBestPriceMatch":true

            var trade = new Trade
            {
                Symbol = "BNBBTC",
                Id = 39316368,
                Price = 0.00395100m,
                Quantity = 20.39000000m,
                BuyerOrderId = -1,
                SellerOrderId = -1,
                Time = new DateTime(2019, 03, 16, 20, 07, 35, 843),
                IsBuyerMaker = true,
                IsBestPriceMatch = true
            };

            return Trades_GetUpdate(new List<Trade> { trade });
        }

        public static List<Trade> Trades_BNB_GetSecondUpdate(List<Trade> previousTrade)
        {
            return Trades_GetUpdate(previousTrade.Skip(5).ToList());
        }

        private static List<Trade> Trades_GetUpdate(List<Trade> previousTrades)
        {
            var trades = new List<Trade>();

            var seed = previousTrades.Count;

            trades.AddRange(previousTrades);

            for (int i = seed; i < 10; i++)
            {
                trades.Add(NewTrade(trades[i - 1]));
            }

            return trades;
        }

        private static Trade NewTrade(Trade previousTrade)
        {
            var random = new Random();

            return new Trade
            {
                Id = previousTrade.Id + 1,
                Price = previousTrade.Price + random.Next(0, 10),
                Quantity = random.Next(100, 5000),
                Time = previousTrade.Time.AddSeconds(1)
            };
        }
    }
}
