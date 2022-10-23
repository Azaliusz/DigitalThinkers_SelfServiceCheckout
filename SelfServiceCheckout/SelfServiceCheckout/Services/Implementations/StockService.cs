using Microsoft.Extensions.Options;
using SelfServiceCheckout.Configurations;
using SelfServiceCheckout.Exceptions;
using SelfServiceCheckout.Models;
using SelfServiceCheckout.Services.Abstractions;

namespace SelfServiceCheckout.Services.Implementations
{
    public class StockService : IStockService
    {
        private readonly MoneyOptions _moneyOptions;

        public StockService(IOptions<MoneyOptions> options)
        {
            _moneyOptions = options.Value;
        }

        public void MoneyDenominationsAddingValidation(Dictionary<int, int> loadedMoneyDenominations, Currencies currency)
        {
            int[] currencyGroup = GetCurrencyGroup(currency);

            foreach (var denomination in loadedMoneyDenominations)
            {
                if (!currencyGroup.Contains(denomination.Key))
                {
                    throw new UnsupportedDenominationException(denomination.Key);
                }

                if (denomination.Value <= 0)
                {
                    throw new UnacceptableDenominationCountException(denomination.Key, denomination.Value);
                }
            }
        }

        private int[] GetCurrencyGroup(Currencies currency)
        {
            return _moneyOptions?.AcceptableDenominations?.ContainsKey(currency) == true
                ? _moneyOptions?.AcceptableDenominations[currency]
                : throw new UndefinedCurrencyException(currency);
        }
    }
}