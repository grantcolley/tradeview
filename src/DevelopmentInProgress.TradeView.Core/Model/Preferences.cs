using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Model
{
    public class Preferences
    {
        public Preferences()
        {
            FavouriteSymbols = new List<string>();
        }

        public string SelectedSymbol { get; set; }
        public bool ShowAggregateTrades { get; set; }
        public int TradeLimit { get; set; }
        public int TradesChartDisplayCount { get; set; }
        public int TradesDisplayCount { get; set; }
        public int OrderBookLimit { get; set; }
        public int OrderBookChartDisplayCount { get; set; }
        public int OrderBookDisplayCount { get; set; }
        public List<string> FavouriteSymbols { get; set; }
    }
}
