using Microsoft.Extensions.Options;
using SelfServiceCheckout.Configurations;
using SelfServiceCheckout.Exceptions;
using SelfServiceCheckout.Models;
using SelfServiceCheckout.Repositories.Abstractions;
using SelfServiceCheckout.Services.Abstractions;

namespace SelfServiceCheckout.Services.Implementations
{
    public class StockService : IStockService
    {
        private readonly IMoneyDenominationRepository _moneyDenominationRepository;
        private readonly MoneyOptions _moneyOptions;

        public StockService(IMoneyDenominationRepository moneyDenominationRepository, IOptions<MoneyOptions> options)
        {
            _moneyDenominationRepository = moneyDenominationRepository;
            _moneyOptions = options.Value;
        }

        public async Task<Dictionary<int, int>> LoadMoneyDenominations(Dictionary<int, int> loadedMoneyDenominations)
        {
            if (loadedMoneyDenominations == null)
            {
                throw new ArgumentNullException(nameof(loadedMoneyDenominations));
            }

            // Validating the incoming data and throw exceptions when they incorrect.
            MoneyDenominationsAddingValidation(loadedMoneyDenominations, _moneyOptions.DefaultCurrency);

            // Storing of incoming data in the repository
            await StoreLoadedDenominations(loadedMoneyDenominations, _moneyOptions.DefaultCurrency);

            // Querying the stored denomination
            return await GetCurrentBalance();
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

        public async Task<Dictionary<int, int>> GetCurrentBalance()
        {
            var actulaMoneyDenomination = await _moneyDenominationRepository.GetDenominationsForCurrencyAsync(_moneyOptions.DefaultCurrency);

            return actulaMoneyDenomination.ToDictionary(
                  moneyDenomination => moneyDenomination.Denomination,
                  moneyDenomination => moneyDenomination.Count);
        }

        public async Task StoreLoadedDenominations(Dictionary<int, int> loadedMoneyDenominations, Currencies currency)
        {
            foreach (var moneyDenomination in loadedMoneyDenominations)
            {
                var foundendMoneyDenomination = await _moneyDenominationRepository.GetAsync(_moneyOptions.DefaultCurrency, moneyDenomination.Key);

                if (foundendMoneyDenomination == null)
                {
                    await _moneyDenominationRepository.AddAsync(new()
                    {
                        Currency = currency,
                        Denomination = moneyDenomination.Key,
                        Count = moneyDenomination.Value
                    });
                }
                else
                {
                    foundendMoneyDenomination.Count += moneyDenomination.Value;
                    await _moneyDenominationRepository.UpdateAsync(foundendMoneyDenomination);
                }
            }
        }

        private int[] GetCurrencyGroup(Currencies currency)
        {
            return _moneyOptions?.AcceptableDenominations?.ContainsKey(currency) == true
                ? _moneyOptions.AcceptableDenominations[currency]
                : throw new UndefinedCurrencyException(currency);
        }
    }
}