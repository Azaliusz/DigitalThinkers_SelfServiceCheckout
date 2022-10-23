using Microsoft.EntityFrameworkCore;
using SelfServiceCheckout.Data;
using SelfServiceCheckout.Models;
using SelfServiceCheckout.Repositories.Abstractions;

namespace SelfServiceCheckout.Repositories.Implementations
{
    public class MoneyDenominationRepository : IMoneyDenominationRepository
    {
        private readonly SelfServiceCheckoutContext _context;

        public MoneyDenominationRepository(SelfServiceCheckoutContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MoneyDenomination>> GetDenominationsForCurrencyAsync(Currencies currency)
        {
            return await _context.MoneyDenominations
                .Where(moneyDenomination => moneyDenomination.Currency == currency)
                .ToListAsync();
        }
    }
}