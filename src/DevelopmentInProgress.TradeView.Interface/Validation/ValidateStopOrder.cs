using DevelopmentInProgress.TradeView.Interface.Model;

namespace DevelopmentInProgress.TradeView.Interface.Validation
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
