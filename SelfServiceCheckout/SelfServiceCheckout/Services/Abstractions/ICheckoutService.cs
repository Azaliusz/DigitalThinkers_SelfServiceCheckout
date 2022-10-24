using SelfServiceCheckout.Models;

namespace SelfServiceCheckout.Services.Abstractions
{
    public interface ICheckoutService
    {
        Task<Dictionary<int, int>> PaymentAndReturns(CheckoutPay checkoutPay);

        Dictionary<int, int>? CalculateChangeDenomination(
                    IEnumerable<MoneyDenomination> virtualBalance,
                    int roundedChangeAmount);
    }
}