using DevelopmentInProgress.MarketView.Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentInProgress.MarketView.Test.Helper
{
    public static class TradesUpdateHelper
    {
        public static List<Trade> Trades_BNB_InitialTradeUpdate_10_Trades()
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

            return Trades_GetUpdate(new List<Trade> { trade }, 9);
        }

        /// <summary>
        /// Gets a new update trades list based on the previous trades list.
        /// Some trades from the previous trade update are kept and new ones added to it.
        /// </summary>
        /// <param name="previousTrades">The previous trade update list.</param>
        /// <param name="skipPreviousTradesCount">The number of old trades to skip when creating the new updated trades list.</param>
        /// <param name="newTradesCount">The number of new trades to create for the updates trades list. They get appended to the old trades that weren't skipped.</param>
        /// <returns></returns>
        public static List<Trade> Trades_BNB_NextTradeUpdate(List<Trade> previousTrades, int skipPreviousTradesCount, int newTradesCount)
        {
            return Trades_GetUpdate(previousTrades.Skip(skipPreviousTradesCount).ToList(), newTradesCount);
        }

        private static List<Trade> Trades_GetUpdate(List<Trade> seedTrades, int newTradesCount)
        {
            var trades = new List<Trade>();

            var seedCount = seedTrades.Count;

            trades.AddRange(seedTrades);

            for (int i = seedCount; i < newTradesCount; i++)
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
