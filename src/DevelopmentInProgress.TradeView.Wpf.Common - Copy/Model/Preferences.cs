using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class Preferences : EntityBase
    {
        public Preferences()
        {
            FavouriteSymbols = new ObservableCollection<string>();
        }

        public string SelectedSymbol { get; set; }
        public bool ShowAggregateTrades { get; set; }
        public int TradeLimit { get; set; }
        public int TradesChartDisplayCount { get; set; }
        public int TradesDisplayCount { get; set; }
        public int OrderBookLimit { get; set; }
        public int OrderBookChartDisplayCount { get; set; }
        public int OrderBookDisplayCount { get; set; }
        public ObservableCollection<string> FavouriteSymbols { get; set; }
    }
}