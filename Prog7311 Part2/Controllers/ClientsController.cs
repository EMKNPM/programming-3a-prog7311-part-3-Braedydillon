using Microsoft.AspNetCore.Mvc;
using Prog7311_Part2.Models;
using System.Net.Http.Json;

namespace Prog7311_Part2.Controllers
{
    public class ClientsController : Controller
    {
        private readonly HttpClient _client;

        public ClientsController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("ApiClient");
        }

        // GET: Clients
        public async Task<IActionResult> Index(string searchString)
        {
            var clients = await _client.GetFromJsonAsync<List<Client>>(
                $"api/clients?searchString={searchString}");

            return View(clients);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var client = await _client.GetFromJsonAsync<Client>(
                $"api/clients/{id}");

            if (client == null)
                return NotFound();

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ClientId,Name,ContactDetails,Region")]
            Client client)
        {
            if (ModelState.IsValid)
            {
                var response =
                    await _client.PostAsJsonAsync(
                        "api/clients",
                        client);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var client =
                await _client.GetFromJsonAsync<Client>(
                    $"api/clients/{id}");

            if (client == null)
                return NotFound();

            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ClientId,Name,ContactDetails,Region")]
            Client client)
        {
            if (id != client.ClientId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var response =
                    await _client.PutAsJsonAsync(
                        $"api/clients/{client.ClientId}",
                        client);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
            }

            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var client =
                await _client.GetFromJsonAsync<Client>(
                    $"api/clients/{id}");

            if (client == null)
                return NotFound();

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _client.DeleteAsync(
                $"api/clients/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}