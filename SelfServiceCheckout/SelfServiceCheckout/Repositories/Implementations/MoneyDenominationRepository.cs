using Microsoft.EntityFrameworkCore;
using SelfServiceCheckout.Data;
using SelfServiceCheckout.Models;
using SelfServiceCheckout.Repositories.Abstractions;

namespace SelfServiceCheckout.Repositories.Implementations
{
    public class MoneyDenominationRepository : GenericRepository<MoneyDenomination, SelfServiceCheckoutDbContext>, IMoneyDenominationRepository
    {
        public MoneyDenominationRepository(SelfServiceCheckoutDbContext context) :
            base(context)
        {
        }

        public Task DeleteAsync(Currencies currency, int denomination)
        {
            return base.DeleteAsync(currency, denomination);
        }

        public Task<bool> Exists(Currencies currency, int denomination)
        {
            return base.Exists(currency, denomination);
        }

        public Task<MoneyDenomination?> GetAsync(Currencies currency, int denomination)
        {
            return base.GetAsync(currency, denomination);
        }

        public async Task<IEnumerable<MoneyDenomination>> GetDenominationsForCurrencyAsync(Currencies currency)
        {
            return await _context.Set<MoneyDenomination>()
                .Where(moneyDenomination => moneyDenomination.Currency == currency)
                .ToListAsync();
        }
    }
}