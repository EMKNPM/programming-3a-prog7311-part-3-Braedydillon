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
            _client = factory.CreateClient("ApiClient");
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index(
            DateOnly? startDate,
            DateOnly? endDate,
            ContractStatus? status)
        {
            var contracts = await _client.GetFromJsonAsync<List<Contract>>(
                $"api/contracts?startDate={startDate}&endDate={endDate}&status={status}");

            return View(contracts);
        }

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

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Contract contract,
            IFormFile? contractFile)
        {
            ModelState.Remove("Client");
            ModelState.Remove("ServiceRequests");

            if (contractFile != null && contractFile.Length > 0)
            {
                var extension = Path.GetExtension(contractFile.FileName).ToLower();

                if (extension != ".pdf")
                {
                    ModelState.AddModelError("DocumentPath", "Only PDF files are allowed.");
                    await PopulateDropdowns(contract.ClientId);
                    return View(contract);
                }

                string fileName =
                    Guid.NewGuid() + "_" +
                    Path.GetFileName(contractFile.FileName);

                string folder =
                    Path.Combine(
                        _hostEnvironment.WebRootPath,
                        "Contracts");

                Directory.CreateDirectory(folder);

                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await contractFile.CopyToAsync(stream);
                }

                contract.DocumentPath = fileName;
            }

            if (ModelState.IsValid)
            {
                var response = await _client.PostAsJsonAsync(
                    "api/contracts",
                    contract);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", "API failed to create contract.");
            }

            await PopulateDropdowns(contract.ClientId);
            return View(contract);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _client.GetFromJsonAsync<Contract>(
                $"api/contracts/{id.Value}");

            if (contract == null)
                return NotFound();

            await PopulateDropdowns(contract.ClientId);
            return View(contract);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            Contract contract,
            IFormFile? contractFile)
        {
            if (id != contract.ContractId)
                return NotFound();

            ModelState.Remove("Client");
            ModelState.Remove("ServiceRequests");

            if (contractFile != null && contractFile.Length > 0)
            {
                var extension = Path.GetExtension(contractFile.FileName).ToLower();

                if (extension != ".pdf")
                {
                    ModelState.AddModelError("DocumentPath", "Only PDF files are allowed.");
                    await PopulateDropdowns(contract.ClientId);
                    return View(contract);
                }

                string fileName =
                    Guid.NewGuid() + "_" +
                    Path.GetFileName(contractFile.FileName);

                string folder =
                    Path.Combine(
                        _hostEnvironment.WebRootPath,
                        "Contracts");

                Directory.CreateDirectory(folder);

                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await contractFile.CopyToAsync(stream);
                }

                contract.DocumentPath = fileName;
            }

            if (ModelState.IsValid)
            {
                var response = await _client.PutAsJsonAsync(
                    $"api/contracts/{contract.ContractId}",
                    contract);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", "API failed to update contract.");
            }

            await PopulateDropdowns(contract.ClientId);
            return View(contract);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _client.GetFromJsonAsync<Contract>(
                $"api/contracts/{id.Value}");

            if (contract == null)
                return NotFound();

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _client.DeleteAsync($"api/contracts/{id}");
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(int? selectedId = null)
        {
            var clients = await _client.GetFromJsonAsync<List<Client>>(
                "api/clients");

            ViewData["ClientId"] = new SelectList(
                clients,
                "ClientId",
                "Name",
                selectedId);

            ViewBag.StatusOptions =
                Enum.GetValues(typeof(ContractStatus))
                    .Cast<ContractStatus>()
                    .Select(s => new SelectListItem
                    {
                        Text = s.ToString(),
                        Value = s.ToString()
                    })
                    .ToList();
        }
    }
}