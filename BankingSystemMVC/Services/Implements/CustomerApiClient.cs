using BankingSystemMVC.Models.Customers;
using BankingSystemMVC.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BankingSystemMVC.Services.Implements
{
    public class CustomerApiClient : ICustomerApiClient
    {
        private readonly HttpClient _client;

        public CustomerApiClient(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("CustomerApi");
        }

        public async Task<CustomerMeViewModel?> GetMeAsync()
        {
            var response = await _client.GetAsync("api/customers/me");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<CustomerMeViewModel>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
    }

}
