using DevelopmentInProgress.MarketView.Interface.Extensions;
using DevelopmentInProgress.MarketView.Interface.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevelopmentInProgress.MarketView.Interface.Validation
{
    public class ClientOrderValidation
    {
        List<Action<Symbol, ClientOrder, StringBuilder>> validationActions;

        public ClientOrderValidation(List<Action<Symbol, ClientOrder, StringBuilder>> validationActions)
        {
            this.validationActions = validationActions;
        }

        public bool TryValidate(Symbol symbol, ClientOrder clientOrder, out string message)
        {
            message = string.Empty;
            StringBuilder messageBuilder = new StringBuilder();

            foreach (var validationAction in validationActions)
            {
                validationAction(symbol, clientOrder, messageBuilder);
            }

            if (messageBuilder.Length > 0)
            {
                message = messageBuilder.ToString();
                message = message.Insert(0, $"{clientOrder.Symbol} {clientOrder.Type.GetOrderTypeName()} order not valid: ");
                if (message.EndsWith(";"))
                {
                    message = message.Remove(message.Length - 1, 1);
                }

                return false;
            }

            return true;
        }
    }
}
