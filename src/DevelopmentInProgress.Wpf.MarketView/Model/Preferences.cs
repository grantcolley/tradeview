using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.MarketView.Model
{
    public class Preferences
    {
        public bool ShowFavourites { get; set; }
        public string SelectedSymbol { get; set; }
        public int TradeLimit { get; set; }
        public int TradesChartDisplayCount { get; set; }
        public int TradesDisplayCount { get; set; }
        public bool UseAggregateTrades { get; set; }
        public int OrderBookLimit { get; set; }
        public int OrderBookChartDisplayCount { get; set; }
        public int OrderBookDisplayCount { get; set; }
        public List<string> FavouriteSymbols { get; set; }
    }
}