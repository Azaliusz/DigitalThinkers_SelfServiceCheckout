using SelfServiceCheckout.Models;
using SelfServiceCheckout.Exceptions;

namespace SelfServiceCheckout.Services.Abstractions
{
    public interface IStockService
    {
        /// <summary>
        /// Decides whether the specified denomination and quantity pairs are acceptable in the given currency.
        /// </summary>
        /// <param name="loadedMoneyDenominations">Dictionary of denomination and quantity pairs.</param>
        /// <param name="currency">The selected currency</param>
        /// <exception cref="UnsupportedDenominationException">
        /// Throw if one of the Denomination is not supported in the specified currency.
        /// </exception>
        /// <exception cref="UnacceptableDenominationCountException">
        /// Throw if one of the Denomination quantity is negative or 0.
        /// </exception>
        void MoneyDenominationsAddingValidation(Dictionary<int, int> loadedMoneyDenominations, Currencies currency);
    }
}