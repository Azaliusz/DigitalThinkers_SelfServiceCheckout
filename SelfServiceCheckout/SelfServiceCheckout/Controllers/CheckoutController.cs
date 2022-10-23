using Microsoft.AspNetCore.Mvc;
using SelfServiceCheckout.Exceptions;
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
            try
            {
                return Ok(await _checkoutService.PaymentAndReturns(checkoutPay));
            }
            catch (SelfServiceCheckoutBaseException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}