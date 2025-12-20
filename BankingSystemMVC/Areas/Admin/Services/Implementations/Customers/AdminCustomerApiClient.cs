using BankingSystemMVC.Areas.Admin.Models;
using BankingSystemMVC.Areas.Admin.Services.Interfaces.Customers;
using BankingSystemMVC.Infrastructure.Json;
using System.Text;
using System.Text.Json;

namespace BankingSystemMVC.Areas.Admin.Services.Implementations.Customers
{
    public class AdminCustomerApiClient : IAdminCustomerApiClient
    {
        private readonly HttpClient _httpClient;

        public AdminCustomerApiClient(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("AdminApi");
        }

        public async Task<List<CustomerListViewModel>?> GetAllCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/customers");
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonHelper.Deserialize<List<CustomerListViewModel>>(json);
            }
            catch
            {
                return null;
            }
        }

        public async Task<CustomerDetailViewModel?> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/admin/customers/{customerId}");
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonHelper.Deserialize<CustomerDetailViewModel>(json);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<AccountSummaryViewModel>?> GetCustomerAccountsAsync(int customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/admin/customers/{customerId}/accounts");
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonHelper.Deserialize<List<AccountSummaryViewModel>>(json);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<TransactionSummaryViewModel>?> GetCustomerTransactionsAsync(int customerId, int limit = 50)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/admin/customers/{customerId}/transactions?limit={limit}");
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonHelper.Deserialize<List<TransactionSummaryViewModel>>(json);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateCustomerStatusAsync(int customerId, string status)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(new { Status = status }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PutAsync($"api/admin/customers/{customerId}/status", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResetCustomerPasswordAsync(int customerId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/admin/customers/{customerId}/reset-password", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<AccountListViewModel>?> GetAllAccountsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/customers/all-accounts");
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonHelper.Deserialize<List<AccountListViewModel>>(json);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<TransactionListViewModel>?> GetAllTransactionsAsync(int limit = 100)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/admin/customers/all-transactions?limit={limit}");
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonHelper.Deserialize<List<TransactionListViewModel>>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
