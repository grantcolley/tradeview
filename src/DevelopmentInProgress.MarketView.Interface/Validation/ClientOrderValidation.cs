using DevelopmentInProgress.MarketView.Interface.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
                message = message.Insert(0, $"{clientOrder.Symbol} {GetOrderTypeName(clientOrder.Type)} not valid: ");
                message = message.Remove(message.Length - 1, 1);
                return false;
            }

            return true;
        }

        private string GetOrderTypeName(OrderType orderType)
        {
            var result = Enum.GetName(typeof(OrderType), orderType);
            return Regex.Replace(result, "[A-Z]", " $0").Trim();
        }
    }
}
