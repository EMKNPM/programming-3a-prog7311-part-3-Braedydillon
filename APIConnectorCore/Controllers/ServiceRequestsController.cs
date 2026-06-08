using APIConnectorCore.Models;
using APIConnectorCore.Repositories;
using APIConnectorCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace APIConnectorCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestRepository _repo;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(
            IServiceRequestRepository repo,
            ICurrencyService currencyService)
        {
            _repo = repo;
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceRequests(
            string? searchString,
            string? status)
        {
            var requests = await _repo.GetFilteredRequestsAsync(
                searchString,
                status);

            return Ok(requests.Select(r => new
            {
                r.ServiceRequestId,
                r.Description,
                r.Status,
                r.ContractId,
                r.Cost
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceRequest(int id)
        {
            var request = await _repo.GetByIdAsync(id);

            if (request == null)
                return NotFound();

            return Ok(new
            {
                request.ServiceRequestId,
                request.Description,
                request.Status,
                request.ContractId,
                request.Cost
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceRequest(
            [FromBody] ServiceRequest request,
            [FromQuery] string fromCurrency = "ZAR")
        {
            request.Cost = await _currencyService.ConvertToZAR(
                request.Cost,
                fromCurrency);

            await _repo.AddAsync(request);
            await _repo.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetServiceRequest),
                new { id = request.ServiceRequestId },
                new
                {
                    request.ServiceRequestId,
                    request.Description,
                    request.Status,
                    request.ContractId,
                    request.Cost
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRequest(
            int id,
            [FromBody] ServiceRequest request,
            [FromQuery] string fromCurrency = "ZAR")
        {
            if (id != request.ServiceRequestId)
                return BadRequest();

            var existing = await _repo.GetByIdAsync(id);

            if (existing == null)
                return NotFound();

            request.Cost = await _currencyService.ConvertToZAR(
                request.Cost,
                fromCurrency);

            await _repo.UpdateAsync(request);
            await _repo.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var existing = await _repo.GetByIdAsync(id);

            if (existing == null)
                return NotFound();

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();

            return NoContent();
        }
    }
}