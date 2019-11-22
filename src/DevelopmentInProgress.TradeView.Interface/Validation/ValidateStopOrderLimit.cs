using DevelopmentInProgress.TradeView.Interface.Model;

namespace DevelopmentInProgress.TradeView.Interface.Validation
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
