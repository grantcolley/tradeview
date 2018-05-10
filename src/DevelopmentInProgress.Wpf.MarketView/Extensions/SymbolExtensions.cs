using DevelopmentInProgress.Wpf.MarketView.Model;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.MarketView.Extensions
{
    public static class SymbolExtensions
    {
        public static Interface.Symbol GetInterfaceSymbol(this Symbol s)
        {
            return new Interface.Symbol
            {
                NotionalMinimumValue = s.NotionalMinimumValue,
                BaseAsset = s.BaseAsset,
                Price = s.Price,
                Quantity = s.Quantity,
                QuoteAsset = s.QuoteAsset,
                Status = s.Status,
                IsIcebergAllowed = s.IsIcebergAllowed,
                OrderTypes = s.OrderTypes
            };
        }

        public static Symbol GetViewSymbol(this Interface.Symbol s)
        {
            return new Symbol
            {
                NotionalMinimumValue = s.NotionalMinimumValue,
                BaseAsset = s.BaseAsset,
                Price = s.Price,
                Quantity = s.Quantity,
                QuoteAsset = s.QuoteAsset,
                Status = s.Status,
                IsIcebergAllowed = s.IsIcebergAllowed,
                OrderTypes = s.OrderTypes,
                SymbolStatistics = new SymbolStatistics { Symbol = $"{s.BaseAsset.Symbol}{s.QuoteAsset.Symbol}" }
            };
        }
    }
}