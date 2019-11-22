using System;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class SymbolStatistics : EntityBase
    {
        private long firstTradeId;
        private DateTime closeTime;
        private DateTime openTime;
        private decimal quoteVolume;
        private long volume;
        private decimal lowPrice;
        private decimal highPrice;
        private decimal openPrice;
        private decimal askQuantity;
        private decimal askPrice;
        private decimal bidQuantity;
        private decimal bidPrice;
        private decimal lastQuantity;
        private decimal lastPrice;
        private decimal previousClosePrice;
        private decimal weightedAveragePrice;
        private decimal priceChangePercent;
        private decimal priceChange;
        private TimeSpan period;
        private string symbol;
        private long lastTradeId;
        private long tradeCount;
        
        public long FirstTradeId
        {
            get { return firstTradeId; }
            set
            {
                if (firstTradeId != value)
                {
                    firstTradeId = value;
                    OnPropertyChanged("FirstTradeId");
                }
            }
        }

        public DateTime CloseTime
        {
            get { return closeTime; }
            set
            {
                if (closeTime != value)
                {
                    closeTime = value;
                    OnPropertyChanged("CloseTime");
                }
            }
        }

        public DateTime OpenTime
        {
            get { return openTime; }
            set
            {
                if (openTime != value)
                {
                    openTime = value;
                    OnPropertyChanged("OpenTime");
                }
            }
        }

        public decimal QuoteVolume
        {
            get { return quoteVolume; }
            set
            {
                if (quoteVolume != value)
                {
                    quoteVolume = value;
                    OnPropertyChanged("QuoteVolume");
                }
            }
        }

        public long Volume
        {
            get { return volume; }
            set
            {
                if (volume != value)
                {
                    volume = value;
                    OnPropertyChanged("Volume");
                }
            }
        }

        public decimal LowPrice
        {
            get { return lowPrice; }
            set
            {
                if (lowPrice != value)
                {
                    lowPrice = value;
                    OnPropertyChanged("LowPrice");
                }
            }
        }

        public decimal HighPrice
        {
            get { return highPrice; }
            set
            {
                if (highPrice != value)
                {
                    highPrice = value;
                    OnPropertyChanged("HighPrice");
                }
            }
        }

        public decimal OpenPrice
        {
            get { return openPrice; }
            set
            {
                if (openPrice != value)
                {
                    openPrice = value;
                    OnPropertyChanged("OpenPrice");
                }
            }
        }

        public decimal AskQuantity
        {
            get { return askQuantity; }
            set
            {
                if (askQuantity != value)
                {
                    askQuantity = value;
                    OnPropertyChanged("AskQuantity");
                }
            }
        }

        public decimal AskPrice
        {
            get { return askPrice; }
            set
            {
                if (askPrice != value)
                {
                    askPrice = value;
                    OnPropertyChanged("AskPrice");
                }
            }
        }

        public decimal BidQuantity
        {
            get { return bidQuantity; }
            set
            {
                if (bidQuantity != value)
                {
                    bidQuantity = value;
                    OnPropertyChanged("BidQuantity");
                }
            }
        }

        public decimal BidPrice
        {
            get { return bidPrice; }
            set
            {
                if (bidPrice != value)
                {
                    bidPrice = value;
                    OnPropertyChanged("BidPrice");
                }
            }
        }

        public decimal LastQuantity
        {
            get { return lastQuantity; }
            set
            {
                if (lastQuantity != value)
                {
                    lastQuantity = value;
                    OnPropertyChanged("LastQuantity");
                }
            }
        }

        public decimal LastPrice
        {
            get { return lastPrice; }
            set
            {
                if (lastPrice != value)
                {
                    lastPrice = value;
                    OnPropertyChanged("LastPrice");
                }
            }
        }

        public decimal PreviousClosePrice
        {
            get { return previousClosePrice; }
            set
            {
                if (previousClosePrice != value)
                {
                    previousClosePrice = value;
                    OnPropertyChanged("PreviousClosePrice");
                }
            }
        }

        public decimal WeightedAveragePrice
        {
            get { return weightedAveragePrice; }
            set
            {
                if (weightedAveragePrice != value)
                {
                    weightedAveragePrice = value;
                    OnPropertyChanged("WeightedAveragePrice");
                }
            }
        }

        public decimal PriceChangePercent
        {
            get { return priceChangePercent; }
            set
            {
                if (priceChangePercent != value)
                {
                    priceChangePercent = value;
                    OnPropertyChanged("PriceChangePercent");
                }
            }
        }

        public decimal PriceChange
        {
            get { return priceChange; }
            set
            {
                if (priceChange != value)
                {
                    priceChange = value;
                    OnPropertyChanged("PriceChange");
                }
            }
        }

        public TimeSpan Period
        {
            get { return period; }
            set
            {
                if (period != value)
                {
                    period = value;
                    OnPropertyChanged("Period");
                }
            }
        }

        public string Symbol
        {
            get { return symbol; }
            set
            {
                if (symbol != value)
                {
                    symbol = value;
                    OnPropertyChanged("Symbol");
                }
            }
        }

        public long LastTradeId
        {
            get { return lastTradeId; }
            set
            {
                if (lastTradeId != value)
                {
                    lastTradeId = value;
                    OnPropertyChanged("LastTradeId");
                }
            }
        }

        public long TradeCount
        {
            get { return tradeCount; }
            set
            {
                if (tradeCount != value)
                {
                    tradeCount = value;
                    OnPropertyChanged("TradeCount");
                }
            }
        }
    }
}