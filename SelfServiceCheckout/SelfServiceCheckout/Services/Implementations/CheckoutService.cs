using Microsoft.Extensions.Options;
using SelfServiceCheckout.Configurations;
using SelfServiceCheckout.Exceptions;
using SelfServiceCheckout.Models;
using SelfServiceCheckout.Repositories.Abstractions;
using SelfServiceCheckout.Services.Abstractions;

namespace SelfServiceCheckout.Services.Implementations
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IMoneyDenominationRepository _moneyDenominationRepository;
        private readonly IStockService _stockService;
        private readonly MoneyOptions _moneyOptions;

        public CheckoutService(
            IMoneyDenominationRepository moneyDenominationRepository,
            IStockService stockService,
            IOptions<MoneyOptions> options)
        {
            _moneyDenominationRepository = moneyDenominationRepository;
            _stockService = stockService;
            _moneyOptions = options.Value;
        }

        public async Task<object?> PaymentAndReturns(CheckoutPay checkoutPay)
        {
            if (checkoutPay is null)
                throw new ArgumentNullException(nameof(checkoutPay));

            var usedCurrency = checkoutPay.Currency ?? _moneyOptions.DefaultCurrency;

            _stockService.MoneyDenominationsAddingValidation(checkoutPay!.Inserted!, usedCurrency);

            double usedCurrencyValue = GetCurrencyValueInDefaultCurrency(usedCurrency);

            // We calculate the total value based on the added denominations.
            double totalIncome = CalculateTotalIncome(checkoutPay!.Inserted!, usedCurrencyValue);

            if (totalIncome < checkoutPay.Price)
            {
                throw new NotEnoughCoverageException(checkoutPay.Price, totalIncome);
            }

            double changeAmount = totalIncome - checkoutPay.Price;

            // The refund amount is rounded according to the HUF rounding rules.
            int roundedChangeAmount = HUFRound(changeAmount);

            var changeDenominations = await CalculateChangeDenomination(roundedChangeAmount);

            if (changeDenominations == null)
            {
                throw new NotPayableReturnException(roundedChangeAmount);
            }

            await RemoveChangeDenomination(changeDenominations);

            return changeDenominations;
        }

        private async Task<Dictionary<int, int>?> CalculateChangeDenomination(int roundedChangeAmount)
        {
            var denominationsInTheMachine = (await _moneyDenominationRepository.GetDenominationsForCurrencyAsync(_moneyOptions.DefaultCurrency))
                .OrderByDescending(denomination => denomination.Denomination)
                .ToArray();

            int[]? resultCounts = RecursiveChangeDenominationCalculation(denominationsInTheMachine, new int[denominationsInTheMachine.Length], 0, roundedChangeAmount);

            if (CalculateCurrentChangeValue(denominationsInTheMachine, resultCounts) != roundedChangeAmount)
            {
                return null;
            }

            var changeDenominations = new Dictionary<int, int>();
            for (int index = 0; index < denominationsInTheMachine.Length; index++)
            {
                if (resultCounts[index] != 0)
                {
                    changeDenominations.Add(denominationsInTheMachine[index].Denomination, resultCounts[index]);
                }
            }

            return changeDenominations;
        }

        private static int[] RecursiveChangeDenominationCalculation(MoneyDenomination[] denominationsInTheMachine, int[] denominationCounts, int currentDenominationIndex, int targetValue)
        {
            if (denominationsInTheMachine.Length == currentDenominationIndex)
            {
                return denominationCounts;
            }

            for (int denominationCount = 0; denominationCount <= denominationsInTheMachine[currentDenominationIndex].Count; denominationCount++)
            {
                denominationCounts[currentDenominationIndex] = denominationCount;

                int totalValueAfterAddedThisDenomination = CalculateCurrentChangeValue(denominationsInTheMachine, denominationCounts);
                if (totalValueAfterAddedThisDenomination >= targetValue)
                {
                    return denominationCounts;
                }

                int[] newCountArray = RecursiveChangeDenominationCalculation(denominationsInTheMachine, denominationCounts, currentDenominationIndex + 1, targetValue);

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

        private async Task RemoveChangeDenomination(Dictionary<int, int> changeDenominations)
        {
            foreach (var changeDenomination in changeDenominations)
            {
                var foundendChangeDenominations = await _moneyDenominationRepository.GetAsync(_moneyOptions.DefaultCurrency, changeDenomination.Key);
                if (foundendChangeDenominations!.Count > changeDenomination.Value)
                {
                    foundendChangeDenominations.Count -= changeDenomination.Value;
                    await _moneyDenominationRepository.UpdateAsync(foundendChangeDenominations);
                }
                else
                {
                    await _moneyDenominationRepository.DeleteAsync(foundendChangeDenominations.Currency, foundendChangeDenominations.Denomination);
                }
            }
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