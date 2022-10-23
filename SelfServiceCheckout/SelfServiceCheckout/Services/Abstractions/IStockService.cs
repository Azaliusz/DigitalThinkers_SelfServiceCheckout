using SelfServiceCheckout.Models;
using SelfServiceCheckout.Exceptions;

namespace SelfServiceCheckout.Services.Abstractions
{
    public interface IStockService
    {
        /// <summary>
        /// It validates incoming denominations and unit numbers and stores them incrementally.
        /// Use <see cref="MoneyDenominationsAddingValidation"/> for validation.
        /// </summary>
        /// <param name="loadedMoneyDenominations">Dictionary of denomination and quantity pairs.</param>
        /// <returns>
        /// The currently stored denominations of the machine after successful denomination addition.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throw if the <paramref name="loadedMoneyDenominations"/> parameter is <see cref="null"/>.
        /// </exception>
        Task<Dictionary<int, int>> LoadMoneyDenominations(Dictionary<int, int> loadedMoneyDenominations);

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