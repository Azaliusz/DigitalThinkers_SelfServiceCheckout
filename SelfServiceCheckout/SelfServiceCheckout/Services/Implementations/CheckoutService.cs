using Microsoft.Extensions.Options;
using SelfServiceCheckout.Configurations;
using SelfServiceCheckout.Exceptions;
using SelfServiceCheckout.Models;
using SelfServiceCheckout.Repositories.Abstractions;
using SelfServiceCheckout.Services.Abstractions;
using System.Text.Json;

namespace SelfServiceCheckout.Services.Implementations
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IMoneyDenominationRepository _moneyDenominationRepository;
        private readonly IStockService _stockService;
        private readonly ILogger<StockService> _logger;
        private readonly MoneyOptions _moneyOptions;

        public CheckoutService(
            IMoneyDenominationRepository moneyDenominationRepository,
            IStockService stockService,
            IOptions<MoneyOptions> options,
            ILogger<StockService> logger)
        {
            _moneyDenominationRepository = moneyDenominationRepository;
            _stockService = stockService;
            _logger = logger;
            _moneyOptions = options.Value;
        }

        public async Task<Dictionary<int, int>> PaymentAndReturns(CheckoutPay checkoutPay)
        {
            if (checkoutPay is null)
            {
                throw new ArgumentNullException(nameof(checkoutPay));
            }

            if (checkoutPay.Price < 0)
            {
                throw new NegativePriceValueException(checkoutPay.Price);
            }

            var usedCurrency = checkoutPay.Currency ?? _moneyOptions.DefaultCurrency;

            // Validateing the loaded denominations in the given currency
            _stockService.MoneyDenominationsAddingValidation(checkoutPay!.Inserted!, usedCurrency);

            double usedCurrencyValue = GetCurrencyValueInDefaultCurrency(usedCurrency);

            // We calculate the total value based on the added denominations.
            double totalIncome = CalculateTotalIncome(checkoutPay!.Inserted!, usedCurrencyValue);

            _logger.LogInformation($"Total income: {totalIncome}");

            if (totalIncome < checkoutPay.Price)
            {
                throw new NotEnoughCoverageException(checkoutPay.Price, totalIncome);
            }

            double changeAmount = totalIncome - checkoutPay.Price;

            _logger.LogInformation($"Desired return value: {changeAmount}");

            // The refund amount is rounded according to the HUF rounding rules.
            int roundedChangeAmount = HUFRound(changeAmount);

            _logger.LogInformation($"Rounded return value: {roundedChangeAmount}");

            //If you have entered the money in the default currency, we will create a virtual container in which we
            //will collect the coins that have been in the machine so far and the newly inserted coins.
            //If you do not use the default currency, we will only load the coins in the machine.
            var virtualBalance = usedCurrency == _moneyOptions.DefaultCurrency
                ? await GetVirtulBalance(checkoutPay!.Inserted!, _moneyOptions.DefaultCurrency)
                : await _moneyDenominationRepository.GetDenominationsForCurrencyAsync(_moneyOptions.DefaultCurrency);

            var changeDenominations = CalculateChangeDenomination(virtualBalance, roundedChangeAmount);

            _logger.LogInformation($"Change denominations: {JsonSerializer.Serialize(changeDenominations)}");

            // The payment cannot be made
            if (changeDenominations == null)
            {
                throw new NotPayableReturnException(roundedChangeAmount);
            }

            // After determining a successful return, we update the virtual balance.
            var updatedVirtualBalance = UpdateVirtualBalance(virtualBalance, changeDenominations);

            // We update the balance of the machine based on the virtual balance
            await UpdateBalance(updatedVirtualBalance);

            // If you did not use the default currency, we will only add the entered denominations to the machine's storage
            if (usedCurrency != _moneyOptions.DefaultCurrency)
            {
                await _stockService.StoreLoadedDenominations(checkoutPay!.Inserted!, usedCurrency);
            }

            return changeDenominations;
        }

        private async Task UpdateBalance(IEnumerable<MoneyDenomination> updatedVirtualBalance)
        {
            foreach (var denomination in updatedVirtualBalance)
            {
                var foundedDenomination = await _moneyDenominationRepository.GetAsync(_moneyOptions.DefaultCurrency,
                                                                                      denomination.Denomination);
                if (foundedDenomination != null)
                {
                    if (denomination.Count == 0)
                    {
                        await _moneyDenominationRepository.DeleteAsync(foundedDenomination.Currency, foundedDenomination.Denomination);
                    }
                    else if (foundedDenomination.Count != denomination.Count)
                    {
                        foundedDenomination.Count = denomination.Count;
                        await _moneyDenominationRepository.UpdateAsync(foundedDenomination);
                    }
                }
                else
                {
                    if (denomination.Count != 0)
                    {
                        await _moneyDenominationRepository.AddAsync(new()
                        {
                            Currency = _moneyOptions.DefaultCurrency,
                            Denomination = denomination.Denomination,
                            Count = denomination.Count
                        });
                    }
                }
            }
        }

        private async Task<IEnumerable<MoneyDenomination>> GetVirtulBalance(
            Dictionary<int, int> insertedDenominations,
            Currencies currency)
        {
            var denominationsInTheMachine = (await _moneyDenominationRepository.GetDenominationsForCurrencyAsync(currency))
                .ToList();

            foreach (var insertedDenomination in insertedDenominations)
            {
                var foundedDenomination = denominationsInTheMachine
                     .Find(denomination => denomination.Denomination == insertedDenomination.Key);

                if (foundedDenomination == null)
                {
                    denominationsInTheMachine.Add(new()
                    {
                        Currency = currency,
                        Denomination = insertedDenomination.Key,
                        Count = insertedDenomination.Value
                    });
                }
                else
                {
                    foundedDenomination.Count += insertedDenomination.Value;
                }
            }

            return denominationsInTheMachine;
        }

        private static Dictionary<int, int>? CalculateChangeDenomination(
            IEnumerable<MoneyDenomination> virtualBalance,
            int roundedChangeAmount)
        {
            var orderedVirtualDenominationsInTheMachine = virtualBalance.OrderBy(denomination => denomination.Denomination)
                .ToArray();

            int[]? resultCounts = RecursiveChangeDenominationCalculation(
                orderedVirtualDenominationsInTheMachine,
                new int[orderedVirtualDenominationsInTheMachine.Length],
                0,
                roundedChangeAmount);

            if (CalculateCurrentChangeValue(orderedVirtualDenominationsInTheMachine, resultCounts) != roundedChangeAmount)
            {
                return null;
            }

            var changeDenominations = new Dictionary<int, int>();
            for (int index = 0; index < orderedVirtualDenominationsInTheMachine.Length; index++)
            {
                if (resultCounts[index] != 0)
                {
                    changeDenominations.Add(orderedVirtualDenominationsInTheMachine[index].Denomination, resultCounts[index]);
                }
            }

            return changeDenominations;
        }

        private static int[] RecursiveChangeDenominationCalculation(
            MoneyDenomination[] denominationsInTheMachine,
            int[] denominationCounts,
            int currentDenominationIndex,
            int targetValue)
        {
            if (denominationsInTheMachine.Length == currentDenominationIndex)
            {
                return denominationCounts;
            }

            for (int denominationCount = 0; denominationCount <= denominationsInTheMachine[currentDenominationIndex].Count; denominationCount++)
            {
                denominationCounts[currentDenominationIndex] = denominationCount;

                int totalValueAfterAddedThisDenomination = CalculateCurrentChangeValue(denominationsInTheMachine, denominationCounts);
                if (totalValueAfterAddedThisDenomination == targetValue)
                {
                    return denominationCounts;
                }
                else if (totalValueAfterAddedThisDenomination > targetValue)
                {
                    denominationCounts[currentDenominationIndex] = 0;
                    return denominationCounts;
                }

                int[] newCountArray = RecursiveChangeDenominationCalculation(denominationsInTheMachine,
                                                                             denominationCounts,
                                                                             currentDenominationIndex + 1,
                                                                             targetValue);

                if (CalculateCurrentChangeValue(denominationsInTheMachine, newCountArray) == targetValue)
                {
                    return newCountArray;
                }
            }

            denominationCounts[currentDenominationIndex] = 0;
            return denominationCounts;
        }

        private static int CalculateCurrentChangeValue(MoneyDenomination[] denominationsInTheMachine, int[] denominationCounts)
        {
            int result = 0;
            for (int i = 0; i < denominationCounts.Length; i++)
            {
                result += denominationsInTheMachine[i].Denomination * denominationCounts[i];
            }

            return result;
        }

        private static IEnumerable<MoneyDenomination> UpdateVirtualBalance(
            IEnumerable<MoneyDenomination> virtualDenominationsInTheMachine,
            Dictionary<int, int> changeDenominations)
        {
            foreach (var denominations in virtualDenominationsInTheMachine)
            {
                if (changeDenominations.ContainsKey(denominations.Denomination))
                {
                    denominations.Count -= changeDenominations[denominations.Denomination];
                }
            }

            return virtualDenominationsInTheMachine;
        }

        private static int HUFRound(double value)
        {
            int roundedValue = (int)Math.Round(value);
            int hufRoundedValue = roundedValue / 10 * 10;
            switch (roundedValue - hufRoundedValue)
            {
                case 0:
                case 1:
                case 2:
                    // In this case, we round down, so we do not increase the previous value.
                    break;

                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    hufRoundedValue += 5;
                    break;

                case 8:
                case 9:
                    hufRoundedValue += 10;
                    break;
            }

            return hufRoundedValue;
        }

        private static double CalculateTotalIncome(Dictionary<int, int> insertedDenominations, double currencyValueInDefaultValue)
        {
            int totalValue = insertedDenominations.Sum(denomination => denomination.Value * denomination.Key);

            return totalValue * currencyValueInDefaultValue;
        }

        private double GetCurrencyValueInDefaultCurrency(Currencies currency)
        {
            return _moneyOptions?.CurrencyValueInDefaultCurrency?.ContainsKey(currency) == true
                ? _moneyOptions.CurrencyValueInDefaultCurrency[currency]
                : throw new UndefinedCurrencyExchangeExceptions(currency);
        }
    }
}