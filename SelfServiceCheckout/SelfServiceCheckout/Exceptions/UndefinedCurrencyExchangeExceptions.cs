using SelfServiceCheckout.Models;

namespace SelfServiceCheckout.Exceptions
{
    public class UndefinedCurrencyExchangeExceptions : SelfServiceCheckoutBaseException
    {
        public UndefinedCurrencyExchangeExceptions(Currencies currency)
            : base($"The conversion value for the base currency is not defined for the {currency} currency.")
        {
        }
    }
}