using APIConnectorCore.Models;
using APIConnectorCore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIConnectorCore.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _repo;

        public ClientsController(IClientRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetClients(string? searchString)
        {
            var clients = await _repo.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                clients = clients
                    .Where(c =>
                        c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        c.ContactDetails.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        c.Region.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient(int id)
        {
            var client = await _repo.GetByIdAsync(id);

            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            await _repo.AddAsync(client);

            return CreatedAtAction(
                nameof(GetClient),
                new { id = client.ClientId },
                client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(
            int id,
            [FromBody] Client client)
        {
            if (id != client.ClientId)
                return BadRequest();

            await _repo.UpdateAsync(client);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            await _repo.DeleteAsync(id);

            return NoContent();
        }
    }
}