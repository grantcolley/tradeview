using DevelopmentInProgress.TradeView.Core.Model;

namespace DevelopmentInProgress.TradeView.Core.Validation
{
    public class ValidateStopOrderLimit : IValidateClientOrder
    {
        public bool TryValidate(Symbol symbol, ClientOrder clientOrder, out string message)
        {
            var clientOrderValidation = new ClientOrderValidationBuilder()
                //.AddPriceValidation()
                .AddStopPriceValidation()
                .Build();

            return clientOrderValidation.TryValidate(symbol, clientOrder, out message);
        }
    }
}
