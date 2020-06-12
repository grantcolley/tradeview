using DevelopmentInProgress.TradeView.Core.Model;

namespace DevelopmentInProgress.TradeView.Core.Validation
{
    public class ValidateLimit : IValidateClientOrder
    {
        public bool TryValidate(Symbol symbol, ClientOrder clientOrder, out string message)
        {
            var clientOrderValidation = new ClientOrderValidationBuilder()
                //.AddPriceValidation()
                .Build();

            return clientOrderValidation.TryValidate(symbol, clientOrder, out message);
        }
    }
}
