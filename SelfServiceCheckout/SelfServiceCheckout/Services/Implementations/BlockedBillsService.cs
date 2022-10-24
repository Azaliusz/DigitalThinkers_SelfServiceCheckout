using Microsoft.Extensions.Options;
using SelfServiceCheckout.Configurations;
using SelfServiceCheckout.Repositories.Abstractions;
using SelfServiceCheckout.Services.Abstractions;

namespace SelfServiceCheckout.Services.Implementations
{
    public class BlockedBillsService : IBlockedBillsService
    {
        private readonly IMoneyDenominationRepository _moneyDenominationRepository;
        private readonly ICheckoutService _checkoutService;
        private readonly IStockService _stockService;
        private readonly MoneyOptions _moneyOptions;

        public BlockedBillsService(
            IMoneyDenominationRepository moneyDenominationRepository,
            ICheckoutService checkoutService,
            IStockService stockService,
            IOptions<MoneyOptions> options)
        {
            _moneyDenominationRepository = moneyDenominationRepository;
            _checkoutService = checkoutService;
            _stockService = stockService;
            _moneyOptions = options.Value;
        }

        public async Task<int[]> GetBlockedBills()
        {
            int[]? currencyGroup = _stockService.GetCurrencyGroup(_moneyOptions.DefaultCurrency);

            var currentBalance = await _moneyDenominationRepository.GetDenominationsForCurrencyAsync(_moneyOptions.DefaultCurrency);

            var acceptableDenominations = new List<int>();
            for (int index = 0; index < currencyGroup.Length; index++)
            {
                var changeDenominations = _checkoutService.CalculateChangeDenomination(currentBalance, currencyGroup[index]);
                if (changeDenominations != null)
                {
                    acceptableDenominations.Add(currencyGroup[index]);
                }
            }

            return acceptableDenominations.ToArray();
        }
    }
}