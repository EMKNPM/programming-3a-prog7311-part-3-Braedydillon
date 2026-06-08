using System.Net;
using Xunit;

namespace Prog7311_UnitTests
{
    public class ApiIntegrationTests
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:7163/")
            };
        }

        [Fact]
        public async Task GetContracts_ReturnsOk_AndJsonIsNotNull()
        {
            var response = await _client.GetAsync("api/contracts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            Assert.False(string.IsNullOrWhiteSpace(json));
        }

        [Fact]
        public async Task GetClients_ReturnsOk_AndJsonIsNotNull()
        {
            var response = await _client.GetAsync("api/clients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            Assert.False(string.IsNullOrWhiteSpace(json));
        }

        [Fact]
        public async Task GetServiceRequests_ReturnsOk_AndJsonIsNotNull()
        {
            var response = await _client.GetAsync("api/servicerequests");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            Assert.False(string.IsNullOrWhiteSpace(json));
        }
    }
}