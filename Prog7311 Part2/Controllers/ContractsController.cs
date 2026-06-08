using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Prog7311_Part2.Models;
using System.Net.Http.Json;

namespace Prog7311_Part2.Controllers
{
    public class ContractsController : Controller
    {
        private readonly HttpClient _client;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ContractsController(
            IHttpClientFactory factory,
            IWebHostEnvironment hostEnvironment)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("http://apiconnectorcore:8080/");

            _hostEnvironment = hostEnvironment;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(
            DateTime? start,
            DateTime? end,
            ContractStatus? status)
        {
            var contracts = await _client.GetFromJsonAsync<List<Contract>>(
                $"api/contracts?start={start}&end={end}&status={status}");

            return View(contracts);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _client.GetFromJsonAsync<Contract>(
                $"api/contracts/{id.Value}");

            if (contract == null)
                return NotFound();

            return View(contract);
        }

        // GET: Contracts/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Contract contract,
            IFormFile? contractFile)
        {
            if (contractFile != null && contractFile.Length > 0)
            {
                string fileName =
                    Guid.NewGuid() + "_" +
                    Path.GetFileName(contractFile.FileName);

                string folder =
                    Path.Combine(
                        _hostEnvironment.WebRootPath,
                        "Contracts");

                Directory.CreateDirectory(folder);

                string path =
                    Path.Combine(folder, fileName);

                using (var stream =
                    new FileStream(path, FileMode.Create))
                {
                    await contractFile.CopyToAsync(stream);
                }

                contract.DocumentPath = fileName;
            }

            ModelState.Remove("Client");

            if (ModelState.IsValid)
            {
                var response =
                    await _client.PostAsJsonAsync(
                        "api/contracts",
                        contract);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(contract.ClientId);

            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var contract =
                await _client.GetFromJsonAsync<Contract>(
                    $"api/contracts/{id.Value}");

            if (contract == null)
                return NotFound();

            await PopulateDropdowns(contract.ClientId);

            return View(contract);
        }

        // POST: Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ContractId,StartDate,EndDate,Status,Servicelevel,ClientId,DocumentPath")]
            Contract contract)
        {
            if (id != contract.ContractId)
                return NotFound();

            ModelState.Remove("Client");

            if (ModelState.IsValid)
            {
                var response =
                    await _client.PutAsJsonAsync(
                        $"api/contracts/{contract.ContractId}",
                        contract);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(contract.ClientId);

            return View(contract);
        }

        private async Task PopulateDropdowns(
            int? selectedId = null)
        {
            var clients =
                await _client.GetFromJsonAsync<List<Client>>(
                    "api/clients");

            ViewData["ClientId"] =
                new SelectList(
                    clients,
                    "ClientId",
                    "Name",
                    selectedId);

            var statusList =
                Enum.GetValues(typeof(ContractStatus))
                    .Cast<ContractStatus>()
                    .Select(s => new SelectListItem
                    {
                        Text = s.ToString(),
                        Value = ((int)s).ToString()
                    })
                    .ToList();

            ViewBag.StatusOptions = statusList;
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var contract =
                await _client.GetFromJsonAsync<Contract>(
                    $"api/contracts/{id.Value}");

            if (contract == null)
                return NotFound();

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _client.DeleteAsync(
                $"api/contracts/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}