using SelfServiceCheckout.Models;

namespace SelfServiceCheckout.Exceptions
{
    public class UndefinedCurrencyException : SelfServiceCheckoutBaseException
    {
        public UndefinedCurrencyException(Currencies currency) : base($"The possible denominations for the {currency} currency are not defined.")
        {
        }
    }
}