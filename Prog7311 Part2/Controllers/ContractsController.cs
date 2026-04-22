using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prog7311_Part2.Models;
using Prog7311_Part2.Repositories;

namespace Prog7311_Part2.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IContractRepository _repo;
        private readonly IClientRepository _clientRepo; // Added this
        private readonly IWebHostEnvironment _hostEnvironment;

        // DB context is now GONE from the constructor
        public ContractsController(IContractRepository repo, IClientRepository clientRepo, IWebHostEnvironment hostEnvironment)
        {
            _repo = repo;
            _clientRepo = clientRepo;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(DateTime? start, DateTime? end, ContractStatus? status)
        {
            var data = await _repo.GetFilteredAsync(start, end, status);
            return View(data);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var contract = await _repo.GetByIdAsync(id.Value);
            return contract == null ? NotFound() : View(contract);
        }

        // GET: Contracts/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile? contractFile)
        {
            if (contractFile != null && contractFile.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(contractFile.FileName);
                string path = Path.Combine(_hostEnvironment.WebRootPath, "Contracts", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await contractFile.CopyToAsync(stream);
                }
                contract.DocumentPath = fileName;
            }

            ModelState.Remove("Client");

            if (ModelState.IsValid)
            {
                await _repo.AddAsync(contract);
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var contract = await _repo.GetByIdAsync(id.Value);
            if (contract == null) return NotFound();

            await PopulateDropdowns(contract.ClientId);
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,StartDate,EndDate,Status,Servicelevel,ClientId,DocumentPath")] Contract contract)
        {
            if (id != contract.ContractId) return NotFound();

            ModelState.Remove("Client");

            if (ModelState.IsValid)
            {
                try
                {
                    await _repo.UpdateAsync(contract);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_repo.ContractExists(contract.ContractId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns(contract.ClientId);
            return View(contract);
        }

        // HELPER: No direct DB access, uses Client Repository
        private async Task PopulateDropdowns(int? selectedId = null)
        {
            var clients = await _clientRepo.GetAllAsync();
            ViewData["ClientId"] = new SelectList(clients, "ClientId", "Name", selectedId);

            var statusList = Enum.GetValues(typeof(ContractStatus))
                                 .Cast<ContractStatus>()
                                 .Select(s => new SelectListItem
                                 {
                                     Text = s.ToString(),
                                     Value = ((int)s).ToString()
                                 }).ToList();

            ViewBag.StatusOptions = statusList;
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var contract = await _repo.GetByIdAsync(id.Value);
            return contract == null ? NotFound() : View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}