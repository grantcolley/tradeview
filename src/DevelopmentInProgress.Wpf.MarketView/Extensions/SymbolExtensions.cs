using DevelopmentInProgress.Wpf.MarketView.Model;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.MarketView.Extensions
{
    public static class SymbolExtensions
    {
        public static Interface.Symbol GetInterfaceSymbol(this Symbol s)
        {
            var symbol = new Interface.Symbol
            {
                NotionalMinimumValue = s.NotionalMinimumValue,
                IsIcebergAllowed = s.IsIcebergAllowed,
                Price = new Interface.InclusiveRange { Minimum = s.Price.Minimum, Maximum = s.Price.Maximum, Increment = s.Price.Increment },
                Quantity = new Interface.InclusiveRange { Minimum = s.Quantity.Minimum, Maximum = s.Quantity.Maximum, Increment = s.Quantity.Increment },
                OrderTypes = s.OrderTypes
            };

            return symbol;
        }
    }
}
