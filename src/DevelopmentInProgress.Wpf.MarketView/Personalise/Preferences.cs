using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.MarketView.Personalise
{
    public class Preferences
    {
        public bool ShowFavourites { get; set; }
        public string SelectedSymbol { get; set; }
        public List<string> FavouriteSymbols { get; set; }
    }
}