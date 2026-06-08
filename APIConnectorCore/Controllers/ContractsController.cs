using APIConnectorCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIConnectorCore.Controllers
{
  
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly ClientContextDatabase _context;

        public ContractsController(ClientContextDatabase context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetContracts(
        DateOnly? startDate,
        DateOnly? endDate,
        ContractStatus? status)
        {
            var query = _context.Contract
                .Include(c => c.Client)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(c => c.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(c => c.EndDate <= endDate.Value);

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            var contracts = await query.ToListAsync();

            return Ok(contracts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContract(int id)
        {
            var contract = await _context.Contract
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ContractId == id);

            if (contract == null)
                return NotFound();

            return Ok(contract);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContract([FromBody] Contract contract)
        {
            _context.Contract.Add(contract);
            await _context.SaveChangesAsync();

            return Ok(contract);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContract(int id, [FromBody] Contract contract)
        {
            if (id != contract.ContractId)
                return BadRequest();

            _context.Entry(contract).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contract.FindAsync(id);

            if (contract == null)
                return NotFound();

            _context.Contract.Remove(contract);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}