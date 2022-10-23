using Microsoft.AspNetCore.Mvc;
using SelfServiceCheckout.Models;
using SelfServiceCheckout.Repositories.Abstractions;

namespace SelfServiceCheckout.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IMoneyDenominationRepository _moneyDenominationRepository;

        public TestController(IMoneyDenominationRepository moneyDenominationRepository)
        {
            _moneyDenominationRepository = moneyDenominationRepository;
        }

        [HttpGet]
        [Route("getTest")]
        public async Task<IActionResult> GetTest()
        {
            return Ok(await _moneyDenominationRepository.GetDenominationsForCurrencyAsync(Currencies.HUF));
        }

        [HttpPost]
        [Route("getTest")]
        public async Task<IActionResult> GetAddOrUpdate([FromBody] Dictionary<int, int> test)
        {
            foreach (var item in test)
            {
                MoneyDenomination? foundendMoneyDenomination = await _moneyDenominationRepository.GetAsync(Currencies.HUF, item.Key);

                if (foundendMoneyDenomination == null)
                {
                    await _moneyDenominationRepository.AddAsync(new MoneyDenomination
                    {
                        Currency = Currencies.HUF,
                        Denomination = item.Key,
                        Count = item.Value
                    });
                }
                else
                {
                    foundendMoneyDenomination.Count += item.Value;
                    await _moneyDenominationRepository.UpdateAsync(foundendMoneyDenomination);
                }
            }

            return Ok(await _moneyDenominationRepository.GetDenominationsForCurrencyAsync(Currencies.HUF));
        }
    }
}