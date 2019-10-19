using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.Wpf.Common.Model;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.Common.Helpers
{
    public class KucoinOrderBookHelper : IOrderBookHelper
    {
        private IExchangeApi kucoinExchangeApi;

        public KucoinOrderBookHelper(IExchangeApi kucoinExchangeApi)
        {
            this.kucoinExchangeApi = kucoinExchangeApi;
        }

        public OrderBook CreateLocalOrderBookReplayCache(Symbol symbol, Interface.OrderBook orderBook, int orderBookCount)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var snapShot = kucoinExchangeApi.GetOrderBookAsync(symbol.ExchangeSymbol, 0, cancellationTokenSource.Token).GetAwaiter().GetResult();

            List<Interface.OrderBookPriceLevel> snapShotAsks;
            List<Interface.OrderBookPriceLevel> snapShotBids;

            if(snapShot.Asks.Count() > orderBookCount)
            {
                snapShotAsks = new List<Interface.OrderBookPriceLevel>(snapShot.Asks);
            }
            else
            {
                snapShotAsks = new List<Interface.OrderBookPriceLevel>(snapShot.Asks);
            }

            if (snapShot.Bids.Count() > orderBookCount)
            {
                snapShotBids = new List<Interface.OrderBookPriceLevel>(snapShot.Bids);
            }
            else
            {
                snapShotBids = new List<Interface.OrderBookPriceLevel>(snapShot.Bids);
            }

            long latestSquence = snapShot.LastUpdateId;

            orderBook.Asks = GetPriceLevels(snapShotAsks, orderBook.Asks, snapShot.LastUpdateId, true, orderBookCount, ref latestSquence);

            orderBook.Bids = GetPriceLevels(snapShotBids, orderBook.Bids, snapShot.LastUpdateId, false, orderBookCount, ref latestSquence);

            return new OrderBook
            {
                LastUpdateId = latestSquence,
                Symbol = orderBook.Symbol,
                BaseSymbol = symbol.BaseAsset.Symbol,
                QuoteSymbol = symbol.QuoteAsset.Symbol
            };
        }

        public void GetBidsAndAsks(Interface.OrderBook orderBook, int pricePrecision, int quantityPrecision, 
            int orderBookCount, int listDisplayCount, int chartDisplayCount, 
            out List<OrderBookPriceLevel> topAsks, out List<OrderBookPriceLevel> topBids, 
            out List<OrderBookPriceLevel> chartAsks, out List<OrderBookPriceLevel> chartBids, 
            out List<OrderBookPriceLevel> aggregatedAsks, out List<OrderBookPriceLevel> aggregatedBids)
        {
            throw new System.NotImplementedException();
        }

        private IEnumerable<Interface.OrderBookPriceLevel> GetPriceLevels(
            List<Interface.OrderBookPriceLevel> snapShotPriceLevels, 
            IEnumerable<Interface.OrderBookPriceLevel> playBackPriceLevels, 
            long snapShotSequence, 
            bool isAsk,
            int orderBookCount, 
            ref long latestSquence)
        {
            var isBid = !isAsk;

            var priceLevels = playBackPriceLevels.Where(a => a.Id > snapShotSequence).ToList();

            foreach (var priceLevel in priceLevels)
            {
                var handled = false;
                var snapShotCount = snapShotPriceLevels.Count;

                if (priceLevel.Id > latestSquence)
                {
                    latestSquence = priceLevel.Id;
                }

                for (int i = 0; i < snapShotCount; i++)
                {
                    var price = priceLevel.Price;

                    if((isAsk && price < snapShotPriceLevels[i].Price)
                        || (isBid && price > snapShotPriceLevels[i].Price))
                    {
                        if (priceLevel.Quantity > 0m)
                        {
                            snapShotPriceLevels.Insert(i, priceLevel);
                        }

                        handled = true;
                        break;
                    }

                    if (price == snapShotPriceLevels[i].Price)
                    {
                        if (priceLevel.Quantity.Equals(0m))
                        {
                            snapShotPriceLevels.Remove(snapShotPriceLevels[i]);
                        }
                        else
                        {
                            snapShotPriceLevels[i].Quantity = priceLevel.Quantity;
                        }

                        handled = true;
                        break;
                    }
                }

                if(handled == false)
                {
                    snapShotPriceLevels.Add(priceLevel);
                }
            }

            if (snapShotPriceLevels.Count >= orderBookCount)
            {
                return snapShotPriceLevels.Take(orderBookCount).ToList();
            }
            else
            {
                return snapShotPriceLevels;
            }
        }
    }
}
