using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Prog7311_Part2.Models;
using System.Net.Http.Json;

namespace Prog7311_Part2.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly HttpClient _client;

        public ServiceRequestsController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("http://apiconnectorcore:8080/");
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index(string searchString, string status)
        {
            var requests =
                await _client.GetFromJsonAsync<List<ServiceRequest>>(
                    $"api/servicerequests?searchString={searchString}&status={status}");

            return View(requests ?? new List<ServiceRequest>());
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var request =
                await _client.GetFromJsonAsync<ServiceRequest>(
                    $"api/servicerequests/{id}");

            if (request == null)
                return NotFound();

            return View(request);
        }

        // GET: ServiceRequests/Create
        public async Task<IActionResult> Create()
        {
            await PopulateContractData();
            GetCurrency();

            return View();
        }

        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ServiceRequest serviceRequest,
            string SourceCurrency)
        {
            if (ModelState.IsValid)
            {
                var response =
                    await _client.PostAsJsonAsync(
                        "api/servicerequests",
                        serviceRequest);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }

            await PopulateContractData();
            GetCurrency();

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var request =
                await _client.GetFromJsonAsync<ServiceRequest>(
                    $"api/servicerequests/{id}");

            if (request == null)
                return NotFound();

            await PopulateContractData();

            return View(request);
        }

        // POST: ServiceRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId)
                return NotFound();

            var response =
                await _client.PutAsJsonAsync(
                    $"api/servicerequests/{id}",
                    serviceRequest);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            await PopulateContractData();

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var request =
                await _client.GetFromJsonAsync<ServiceRequest>(
                    $"api/servicerequests/{id}");

            if (request == null)
                return NotFound();

            return View(request);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _client.DeleteAsync(
                $"api/servicerequests/{id}");

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateContractData()
        {
            var contracts =
                await _client.GetFromJsonAsync<List<Contract>>(
                    "api/contracts");

            ViewBag.ContractList =
                contracts?.Select(c => new SelectListItem
                {
                    Value = c.ContractId.ToString(),
                    Text = $"Contract {c.ContractId}"
                }).ToList();
        }

        private void GetCurrency()
        {
            var currencies =
                System.Globalization.CultureInfo
                .GetCultures(System.Globalization.CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture && c.LCID != 127)
                .Select(c =>
                {
                    try
                    {
                        return new System.Globalization.RegionInfo(c.Name)
                            .ISOCurrencySymbol;
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ViewBag.Currencies = new SelectList(currencies);
        }
    }
}