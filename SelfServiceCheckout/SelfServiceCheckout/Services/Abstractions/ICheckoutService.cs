using SelfServiceCheckout.Models;

namespace SelfServiceCheckout.Services.Abstractions
{
    public interface ICheckoutService
    {
        Task<object?> PaymentAndReturns(CheckoutPay checkoutPay);
    }
}