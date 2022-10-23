using Microsoft.AspNetCore.Mvc;
using SelfServiceCheckout.Exceptions;
using SelfServiceCheckout.Services.Abstractions;

namespace SelfServiceCheckout.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Dictionary<int, int> loadedMoneyDenominations)
        {
            try
            {
                return Ok(await _stockService.LoadMoneyDenominations(loadedMoneyDenominations));
            }
            catch (SelfServiceCheckoutBaseException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}