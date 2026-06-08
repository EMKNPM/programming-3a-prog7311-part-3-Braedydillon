using APIConnectorCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIConnectorCore.Controllers
{
  
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet("convert")]
        public async Task<IActionResult> Convert(
            decimal amount,
            string fromCurrency)
        {
            var result =
                await _currencyService.ConvertToZAR(
                    amount,
                    fromCurrency);

            return Ok(result);
        }
    }
}