using DevelopmentInProgress.TradeView.Core.Model;

namespace DevelopmentInProgress.TradeView.Core.Validation
{
    public class ValidateMarket : IValidateClientOrder
    {
        public bool TryValidate(Symbol symbol, ClientOrder clientOrder, out string message)
        {
            var clientOrderValidation = new ClientOrderValidationBuilder()
                .Build();

            return clientOrderValidation.TryValidate(symbol, clientOrder, out message);
        }
    }
}
