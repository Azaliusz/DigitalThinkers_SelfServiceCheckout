using SelfServiceCheckout.Data;
using SelfServiceCheckout.Models;

namespace SelfServiceCheckout.Repositories.Abstractions
{
    public interface IMoneyDenominationRepository : IGenericRepository<MoneyDenomination, SelfServiceCheckoutContext>
    {
        Task<MoneyDenomination?> GetAsync(Currencies currency, int denomination);

        Task DeleteAsync(Currencies currency, int denomination);

        Task<bool> Exists(Currencies currency, int denomination);

        Task<IEnumerable<MoneyDenomination>> GetDenominationsForCurrencyAsync(Currencies currency);
    }
}