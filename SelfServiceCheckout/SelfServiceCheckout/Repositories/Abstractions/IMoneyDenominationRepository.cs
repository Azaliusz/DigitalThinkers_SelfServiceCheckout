using SelfServiceCheckout.Models;

namespace SelfServiceCheckout.Repositories.Abstractions
{
    public interface IMoneyDenominationRepository
    {
        Task<IEnumerable<MoneyDenomination>> GetDenominationsForCurrencyAsync(Currencies currency);
    }
}