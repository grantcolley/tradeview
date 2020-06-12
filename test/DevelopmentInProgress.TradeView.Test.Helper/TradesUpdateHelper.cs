using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentInProgress.TradeView.Test.Helper
{
    public static class TradesUpdateHelper
    {
        public static List<ITrade> Trades_BNB_InitialTradeUpdate_10_Trades()
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
                Time = new DateTime(2019, 03, 16, 20, 07, 35, 843),
                IsBuyerMaker = true
            };

            return Trades_GetUpdate(new List<ITrade> { trade }, 9);
        }

        /// <summary>
        /// Gets a new update trades list based on the previous trades list.
        /// Some trades from the previous trade update are kept and new ones added to it.
        /// </summary>
        /// <param name="previousTrades">The previous trade update list.</param>
        /// <param name="skipPreviousTradesCount">The number of old trades to skip when creating the new updated trades list.</param>
        /// <param name="newTradesCount">The number of new trades to create for the updates trades list. They get appended to the old trades that weren't skipped.</param>
        /// <returns></returns>
        public static List<ITrade> Trades_BNB_NextTradeUpdate(List<ITrade> previousTrades, int skipPreviousTradesCount, int newTradesCount)
        {
            return Trades_GetUpdate(previousTrades.Skip(skipPreviousTradesCount).ToList(), newTradesCount);
        }

        private static List<ITrade> Trades_GetUpdate(List<ITrade> seedTrades, int newTradesCount)
        {
            var trades = new List<ITrade>();

            var seedCount = seedTrades.Count;

            newTradesCount += seedCount;

            trades.AddRange(seedTrades);

            for (int i = seedCount; i < newTradesCount; i++)
            {
                trades.Add(NewTrade(trades[i - 1]));
            }

            return trades;
        }

        private static Trade NewTrade(ITrade previousTrade)
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
