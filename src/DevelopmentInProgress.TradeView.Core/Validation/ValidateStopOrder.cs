using DevelopmentInProgress.TradeView.Core.Model;

namespace DevelopmentInProgress.TradeView.Core.Validation
{
    public class ValidateStopOrder : IValidateClientOrder
    {
        public bool TryValidate(Symbol symbol, ClientOrder clientOrder, out string message)
        {
            var clientOrderValidation = new ClientOrderValidationBuilder()
                .AddStopPriceValidation()
                .Build();

            return clientOrderValidation.TryValidate(symbol, clientOrder, out message);
        }
    }
}
