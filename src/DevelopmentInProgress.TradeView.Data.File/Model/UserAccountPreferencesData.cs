using DevelopmentInProgress.TradeView.Core.Enums;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Data.File.Model
{
    public class UserAccountPreferencesData
    {
        public UserAccountPreferencesData()
        {
            FavouriteSymbols = new List<string>();
        }

        public string AccountName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string ApiPassPhrase { get; set; }
        public Exchange Exchange { get; set; }
        public string SelectedSymbol { get; set; }
        public bool ShowAggregateTrades { get; set; }
        public int TradeLimit { get; set; }
        public int TradesChartDisplayCount { get; set; }
        public int TradesDisplayCount { get; set; }
        public int OrderBookLimit { get; set; }
        public int OrderBookChartDisplayCount { get; set; }
        public int OrderBookDisplayCount { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Setter required for serializing and deserialising the object to a data file.")]
        public List<string> FavouriteSymbols { get; set; }
    }
}
