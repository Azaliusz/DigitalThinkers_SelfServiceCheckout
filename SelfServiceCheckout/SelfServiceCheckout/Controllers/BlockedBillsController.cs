using Microsoft.AspNetCore.Mvc;
using SelfServiceCheckout.Services.Abstractions;

namespace SelfServiceCheckout.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BlockedBillsController : ControllerBase
    {
        private readonly IBlockedBillsService _blockedBillsService;

        public BlockedBillsController(IBlockedBillsService blockedBillsService)
        {
            _blockedBillsService = blockedBillsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _blockedBillsService.GetBlockedBills());
        }
    }
}