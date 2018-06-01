using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.MarketView.Personalise
{
    public class SymbolPreferences
    {
        public bool ShowFavourites { get; set; }
        public string SelectedSymbol { get; set; }
        public List<string> FavouriteSymbols { get; set; }
    }
}