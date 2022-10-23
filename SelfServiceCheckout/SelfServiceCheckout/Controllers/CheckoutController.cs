using Microsoft.AspNetCore.Mvc;
using SelfServiceCheckout.Models;
using SelfServiceCheckout.Services.Abstractions;

namespace SelfServiceCheckout.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] CheckoutPay checkoutPay)
        {
            return Ok(await _checkoutService.PaymentAndReturns(checkoutPay));
        }
    }
}